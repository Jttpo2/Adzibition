using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExhibitionController : MonoBehaviour
{
	private List<Transform> imageTargets;

	// Use this for initialization
	void Start ()
	{
		imageTargets = getAllImageTargets ();

		Debug.Log (imageTargets);
		for (int i = 0; i < imageTargets.Count; i++) {
			coverWithPlane (imageTargets [i]);
			matchSpriteChildToTarget (imageTargets [i]);
			matchCanvasCubeChildToTarget (imageTargets [i]);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void matchToImageTarget (Transform artPiece, Transform target)
	{
		artPiece.SetParent (target);

		// Center on image target
		artPiece.position = target.position;

		// Scale to image target size
		scaleToTargetWithRatioIntact (artPiece, target);

		// Move in front of target a tiny bit to not come into glitch conflict with image target. Unnecessary?
		artPiece.transform.Translate (new Vector3 (0, 0, -0.1f));

		// Rotate it to stand up, and turn it to face the camera (90 degrees z and 180 degrees x)
		Quaternion standUp = Quaternion.Euler (
			                     new Vector3 (90, 0, 0));
		artPiece.transform.localRotation = standUp;
	}

	private Vector3 getSizeOf (Transform transform)
	{
		return transform.GetComponent <Renderer> ().bounds.size;
	}

	private void scaleToTargetWithRatioIntact (Transform artPiece, Transform target)
	{
		Vector3 artPieceBoundingBox = getSizeOf (artPiece);
		Vector3 targetBoundingBox = getSizeOf (target);

		float artPieceToTargetScalarX = targetBoundingBox.x / artPieceBoundingBox.x;
		float artPieceToTargetScalarY = targetBoundingBox.y / artPieceBoundingBox.y;
		float artPieceToTargetScalarZ = targetBoundingBox.z / artPieceBoundingBox.z;

		float artPieceWidthToHeightRatio = artPieceBoundingBox.x / artPieceBoundingBox.z;
		float targetWidthToHeightRatio = targetBoundingBox.x / targetBoundingBox.z;
	
		Vector3 newLocalScale = Vector3.one;

		if (Mathf.Abs (artPieceWidthToHeightRatio) > 10) {

			Debug.LogError ("Weird width to height ratio, " + target + " ArtPiece: " + artPieceWidthToHeightRatio);
		} else if (
			Mathf.Abs (targetWidthToHeightRatio) > 10) {
			Debug.LogError ("Weird width to height ratio, " + target + " Target: " + artPieceWidthToHeightRatio);
		}
		if (targetWidthToHeightRatio > 1) {
			// Target wider than tall

//			if (artPieceWidthToHeightRatio > 1) {
			// Art piece wider than tall

			newLocalScale = new Vector3 (
				artPiece.localScale.x * artPieceToTargetScalarZ,
				artPiece.localScale.y * artPieceToTargetScalarZ,
				1f);
//			} else {
//				// Art work taller than wide
//				newLocalScale = new Vector3 (
//					artPiece.localScale.x * artPieceToTargetScalarZ,
//					artPiece.localScale.y * artPieceToTargetScalarZ,
//					1f);
//			}
		} else {
			// Target taller than wide
//			if (artPieceWidthToHeightRatio > 1) {
			// Art piece wider than tall
			newLocalScale = new Vector3 (
				artPiece.localScale.x * artPieceToTargetScalarX,
				artPiece.localScale.y * artPieceToTargetScalarX,
				1f);
//			} else {
//				// Art work taller than wide
//				newLocalScale = new Vector3 (
//					artPiece.localScale.x * artPieceToTargetScalarX,
//					artPiece.localScale.y * artPieceToTargetScalarX,
//					1f);
//			}
		}



		artPiece.localScale = newLocalScale;
	}

	private void coverWithPlane (Transform imgTarget)
	{
		// Create plane background
		GameObject plane = GameObject.CreatePrimitive (PrimitiveType.Plane);
		
		// Set image targets parent temporarily as parent, for correct scaling
		plane.transform.SetParent (imgTarget.parent);
		
		// Center on image target
		plane.transform.position = imgTarget.position;
		
		// Rotate it to stand up
//		Quaternion standUp = Quaternion.Euler (
//			                     new Vector3 (-90, 0, 0));
//		plane.transform.localRotation = standUp;

		// Scale to image target size
		fitToTarget (plane.transform, imgTarget.GetComponent <Vuforia.ImageTargetBehaviour> (), 0.1f);
		
		// Move in front of target a tiny bit to not come into glitch conflict with image target. Unnecessary?
//		plane.transform.Translate (new Vector3 (0, 0.0001f, 0));
		
		// Paint it black
		plane.GetComponent <Renderer> ().material.color = Color.black;
		
		// Make it plain (apply a basic shader with no lighting)
		plane.GetComponent <Renderer> ().material.shader = Shader.Find ("Unlit/Color");

		// Set image target as parent
		plane.transform.SetParent (imgTarget);
	}

	private void fitToTarget (Transform fittee, Vuforia.ImageTargetBehaviour imgTarget, float scale)
	{
		// Get image targe size 
		Vector2 imgTargetSize = imgTarget.GetSize ();

		float imgTargetWidth = imgTargetSize.x;
		float imgTargetHeight = imgTargetSize.y;
		
		// Scale plane to image target size
		fittee.transform.localScale = new Vector3 (
			imgTargetWidth * scale, 
			1f * scale, 
			imgTargetHeight * scale);
	}

	private float getScalar (float fittee, float target)
	{
		if (Mathf.Abs (fittee) < 0.01f || Mathf.Abs (target) < 0.01f) {
			return 1f;
		} else {
			return target / fittee;
		}
	}

	private void fitToTransform (Transform fittee, Transform target, float scale, bool keepAspectRatio)
	{
		Quaternion fitteeRotation = fittee.rotation;
		Quaternion targetRotation = target.rotation;

		// Temporarily reset rotation
		fittee.rotation = Quaternion.identity;
		target.rotation = Quaternion.identity;

		Vector3 targetBoundingBox = target.GetComponent <Renderer> ().bounds.size;
		Vector3 fitteeBoundingBox = fittee.GetComponent <Renderer> ().bounds.size;

		float fitteeToTargetScalarX = getScalar (fitteeBoundingBox.x, targetBoundingBox.x);
		float fitteeToTargetScalarY = getScalar (fitteeBoundingBox.y, targetBoundingBox.y);
		float fitteeToTargetScalarZ = getScalar (fitteeBoundingBox.z, targetBoundingBox.z);



		Vector3 newLocalScale = new Vector3 (
			                        fittee.localScale.x * fitteeToTargetScalarX,
			                        fittee.localScale.y * fitteeToTargetScalarY,
			                        fittee.localScale.z * fitteeToTargetScalarZ
		                        );

		fittee.localScale = newLocalScale;

	


//		Vector3 fitteBoundingBox = getSizeOf (fittee);
//		Vector3 targetBoundingBox = getSizeOf (target);
//
//		float fitteeToTargetScalarX = targetBoundingBox.x / fitteBoundingBox.x;
//		float fitteeToTargetScalarY = targetBoundingBox.y / fitteBoundingBox.y;
//		float fitteeToTargetScalarZ = targetBoundingBox.z / fitteBoundingBox.z;
//
//		Vector3 newLocalScale = Vector3.one;
//		if (keepAspectRatio) {
//			float artPieceWidthToHeightRatio = fitteBoundingBox.x / fitteBoundingBox.z;
//			float targetWidthToHeightRatio = targetBoundingBox.x / targetBoundingBox.z;
//
//			if (Mathf.Abs (artPieceWidthToHeightRatio) > 10) {
//
//				Debug.LogError ("Weird width to height ratio, " + target + " ArtPiece: " + artPieceWidthToHeightRatio);
//			} else if (
//				Mathf.Abs (targetWidthToHeightRatio) > 10) {
//				Debug.LogError ("Weird width to height ratio, " + target + " Target: " + artPieceWidthToHeightRatio);
//			}
//
//			if (targetWidthToHeightRatio > 1) {
//				// Target wider than tall
//				newLocalScale = new Vector3 (
//					fittee.localScale.x * fitteeToTargetScalarZ,
//					fittee.localScale.y * fitteeToTargetScalarZ,
//					1f);
//			} else {
//				// Target taller than wide
//				newLocalScale = new Vector3 (
//					fittee.localScale.x * fitteeToTargetScalarX,
//					fittee.localScale.y * fitteeToTargetScalarX,
//					1f);
//			}
//		} else {
//			newLocalScale = new Vector3 (
//				fittee.localScale.x * fitteeToTargetScalarX,
//				fittee.localScale.y * fitteeToTargetScalarY,
//				fittee.localScale.z * fitteeToTargetScalarZ);
//		}
//		fittee.localScale = newLocalScale;

		// Rotate back
		fittee.rotation = fitteeRotation;
		target.rotation = targetRotation;
	}

	private void matchSpriteChildToTarget (Transform imgTarget)
	{
		Transform spriteChild = getFirstSpriteChildOf (imgTarget);
		if (spriteChild) {
			matchToImageTarget (spriteChild, imgTarget);	
		}
	}

	private Transform getFirstSpriteChildOf (Transform imgTarget)
	{
		return imgTarget.GetComponentInChildren <SpriteRenderer> ().transform;
	}

	private void matchCanvasCubeChildToTarget (Transform imgTarget)
	{
		Transform canvas = getCanvasCubeOf (imgTarget);
		if (canvas) {
			Transform sprite = getFirstSpriteChildOf (imgTarget);
			if (sprite) {
				scaleCanvasCubeTo (canvas, sprite);	
			}

		}
	}

	private Transform getCanvasCubeOf (Transform imgTarget)
	{
		return imgTarget.Find ("Canvas").transform;
	}

	private void scaleCanvasCubeTo (Transform canvasCube, Transform artWork)
	{
		Transform main = canvasCube.Find ("Top");
		fitToTransform (main, artWork, 1, false);
	}

	private List<Transform> getAllImageTargets ()
	{
		return Helper.FindComponentsInChildWithTag<Transform> (transform.FindChild ("Dev").gameObject, "ImageTarget");
	}
}
