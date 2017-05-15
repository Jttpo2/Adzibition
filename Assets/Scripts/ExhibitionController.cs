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

	void matchToImageTarget (Transform artWork, Transform target)
	{
		artWork.SetParent (target);

		// Center on image target
		artWork.position = target.position;

		// Scale to image target size
		scaleToTargetWithRatioIntact (artWork, target);

		// Move in front of target a tiny bit to not come into glitch conflict with image target. Unnecessary?
		artWork.transform.Translate (new Vector3 (0, 0, -0.1f));

		// Rotate it to stand up, and turn it to face the camera (90 degrees z and 180 degrees x)
		Quaternion standUp = Quaternion.Euler (
			                     new Vector3 (90, 0, 0));
		artWork.transform.localRotation = standUp;
	}

	private Vector3 getSizeOf (Transform transform)
	{
		return transform.GetComponent <Renderer> ().bounds.size;
	}

	private void scaleToTargetWithRatioIntact (Transform artWork, Transform target)
	{
		Vector3 artWorkBoundingBox = getSizeOf (artWork);
		Vector3 targetBoundingBox = getSizeOf (target);

		float artWorkToTargetScalarX = targetBoundingBox.x / artWorkBoundingBox.x;
		float artWorkToTargetScalarY = targetBoundingBox.y / artWorkBoundingBox.y;
		float artWorkToTargetScalarZ = targetBoundingBox.z / artWorkBoundingBox.z;

		float artWorkWidthToHeightRatio = artWorkBoundingBox.x / artWorkBoundingBox.z;
		float targetWidthToHeightRatio = targetBoundingBox.x / targetBoundingBox.z;
	
		Vector3 newLocalScale = Vector3.one;

		if (Mathf.Abs (artWorkWidthToHeightRatio) > 10) {

			Debug.LogError ("Weird width to height ratio, " + target + " ArtPiece: " + artWorkWidthToHeightRatio);
		} else if (
			Mathf.Abs (targetWidthToHeightRatio) > 10) {
			Debug.LogError ("Weird width to height ratio, " + target + " Target: " + artWorkWidthToHeightRatio);
		}
		if (targetWidthToHeightRatio > 1) {
			// Target wider than tall

//			if (artWorkWidthToHeightRatio > 1) {
			// Art piece wider than tall

			newLocalScale = new Vector3 (
				artWork.localScale.x * artWorkToTargetScalarZ,
				artWork.localScale.y * artWorkToTargetScalarZ,
				1f);
//			} else {
//				// Art work taller than wide
//				newLocalScale = new Vector3 (
//					artWork.localScale.x * artWorkToTargetScalarZ,
//					artWork.localScale.y * artWorkToTargetScalarZ,
//					1f);
//			}
		} else {
			// Target taller than wide
//			if (artWorkWidthToHeightRatio > 1) {
			// Art piece wider than tall
			newLocalScale = new Vector3 (
				artWork.localScale.x * artWorkToTargetScalarX,
				artWork.localScale.y * artWorkToTargetScalarX,
				1f);
//			} else {
//				// Art work taller than wide
//				newLocalScale = new Vector3 (
//					artWork.localScale.x * artWorkToTargetScalarX,
//					artWork.localScale.y * artWorkToTargetScalarX,
//					1f);
//			}
		}



		artWork.localScale = newLocalScale;
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
			// Rotate to suit artwork
			canvas.Rotate (new Vector3 (90, 0, 0));
		
			// Set thickness of canvas
			float thickness = 0.03f;
			canvas.localScale = new Vector3 (canvas.localScale.x, canvas.localScale.y, thickness);

			// Move canvas to on top of background plane
			Vector3 canvasExtents = canvas.GetComponent <Renderer> ().bounds.extents;
			canvas.Translate (new Vector3 (0f, 0f, -canvasExtents.y));

			// Move artWork to on top of canvas
			if (sprite) {
				Vector3 canvasBoundingBox = canvas.GetComponent <Renderer> ().bounds.size;
				sprite.Translate (new Vector3 (0f, 0f, -canvasBoundingBox.y));
			}
		}
	}

	private Transform getCanvasCubeOf (Transform imgTarget)
	{
		return imgTarget.Find ("Canvas").transform;
	}

	private void scaleCanvasCubeTo (Transform canvasCube, Transform artWork)
	{
		fitToTransform (canvasCube, artWork, 1, false);
	}

	private List<Transform> getAllImageTargets ()
	{
		return Helper.FindComponentsInChildWithTag<Transform> (transform.FindChild ("Dev").gameObject, "ImageTarget");
	}
}
