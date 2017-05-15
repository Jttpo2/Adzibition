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

	private void scaleToTargetWithRatioIntact (Transform artPiece, Transform target)
	{
		Vector3 artPieceBoundingBox = artPiece.GetComponent <Renderer> ().bounds.size;
		Vector3 targetBoundingBox = target.GetComponent <Renderer> ().bounds.size;

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
		fitToTarget (plane.transform, imgTarget);
		
		// Move in front of target a tiny bit to not come into glitch conflict with image target. Unnecessary?
//		plane.transform.Translate (new Vector3 (0, 0.0001f, 0));
		
		// Paint it black
		plane.GetComponent <Renderer> ().material.color = Color.black;
		
		// Make it plain (apply a basic shader with no lighting)
		plane.GetComponent <Renderer> ().material.shader = Shader.Find ("Unlit/Color");

		// Set image target as parent
		plane.transform.SetParent (imgTarget);
	}

	private void fitToTarget (Transform fittee, Transform imgTarget)
	{
		// Get image targe size 
		Vector2 imgTargetSize = imgTarget.GetComponent <Vuforia.ImageTargetBehaviour> ().GetSize ();
		float imgTargetWidth = imgTargetSize.x;
		float imgTargetHeight = imgTargetSize.y;
		
		// Scale plane to image target size
		fittee.transform.localScale = new Vector3 (
			imgTargetWidth / 10f, 
			1f / 10f, 
			imgTargetHeight / 10f);
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
			scaleCanvasCubeTo (canvas, imgTarget);
		}
	}

	private Transform getCanvasCubeOf (Transform imgTarget)
	{
		return imgTarget.Find ("Canvas").transform;
	}

	private void scaleCanvasCubeTo (Transform canvasCube, Transform imgTarget)
	{
		Transform main = canvasCube.Find ("Front");
		matchToImageTarget (main, imgTarget);
	}

	private List<Transform> getAllImageTargets ()
	{
		return Helper.FindComponentsInChildWithTag<Transform> (transform.FindChild ("Dev").gameObject, "ImageTarget");
	}
}
