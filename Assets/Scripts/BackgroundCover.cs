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
		plane.transform.SetParent (thisTransform);

		// Center on image target
		plane.transform.position = thisTransform.position;

		// Move in front of target a tiny bit to not come into glitch conflict with image target. Unnecessary?
		plane.transform.Translate (new Vector3 (0, 0.0001f, 0));

		// Scale to image target size
		// TODO: Scale to width of image target if wide, else scale to height
		plane.transform.localScale = new Vector3 (0.1f, 0.1f, 0.1f);
//			thisTransform.localScale * (0.1f);
//			new Vector3 (1, 1, 1);
//		1.8 0.1

		// Rotate it to stand up, and turn it to face the camera (90 degrees z and 180 degrees x)
		Quaternion standUp = Quaternion.Euler (new Vector3 (0, 90, 0));
		plane.transform.localRotation = standUp;

		// Paint it black
		plane.GetComponent <Renderer> ().material.color = Color.black;

		// Make it plain (apply a basic shader with no lighting)
		plane.GetComponent <Renderer> ().material.shader = Shader.Find ("Unlit/Color");

		//		Debug.Log (material.color);


	}
	
	// Update is called once per frame
	void Update ()
	{
//		plane.transform.localScale = 
	}
}
