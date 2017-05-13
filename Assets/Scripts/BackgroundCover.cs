using UnityEngine;
using System.Collections;

// For instantiating a simple black background covering the image target fully
public class BackgroundCover : MonoBehaviour
{
	private Transform thisTransform;
	private GameObject plane;

	void Start ()
	{
		thisTransform = transform;

		// Create plane background
		plane = GameObject.CreatePrimitive (PrimitiveType.Plane);

		// Set image target as parent
//		plane.transform.SetParent (thisTransform);

		// Set image targets parent as parent, for correct scaling
		plane.transform.SetParent (thisTransform.parent);

		// Center on image target
		plane.transform.position = thisTransform.position;

		// Rotate it to stand up, and turn it to face the camera (90 degrees z and 180 degrees x)
		Quaternion standUp = Quaternion.Euler (new Vector3 (0, 90, 0));
		plane.transform.localRotation = standUp;

		// Scale to image target size
		sizeToImgTarget ();

		// Move in front of target a tiny bit to not come into glitch conflict with image target. Unnecessary?
		plane.transform.Translate (new Vector3 (0, -0.1f, 0));

		// TODO: Scale to width of image target if wide, else scale to height

		// Paint it black
		plane.GetComponent <Renderer> ().material.color = Color.black;

		// Make it plain (apply a basic shader with no lighting)
		plane.GetComponent <Renderer> ().material.shader = Shader.Find ("Unlit/Color");
	}


	private void sizeToImgTarget ()
	{
		// Get image targe size 
		Vector2 imgTargetSize = GetComponent <Vuforia.ImageTargetBehaviour> ().GetSize ();
		float imgTargetWidth = imgTargetSize.x;
		float imgTargetHeight = imgTargetSize.y;

		// Scale plane to image target size
		plane.transform.localScale = new Vector3 (imgTargetHeight / 10f, 0.1f, imgTargetWidth / 10f);
	}
}
