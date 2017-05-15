using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackableList : MonoBehaviour
{

	// Update is called once per frame
	void Update ()
	{
		// Get the Vuforia StateManager
		Vuforia.StateManager sm = Vuforia.TrackerManager.Instance.GetStateManager ();

		// Query the StateManager to retrieve the list of
		// currently 'active' trackables 
		//(i.e. the ones currently being tracked by Vuforia)
		IEnumerable<Vuforia.TrackableBehaviour> activeTrackables = sm.GetActiveTrackableBehaviours ();

		// Iterate through the list of active trackables
		Debug.Log ("List of trackables currently active (tracked): ");
		foreach (Vuforia.TrackableBehaviour tb in activeTrackables) {
			Debug.Log ("Trackable: " + tb.TrackableName);
		}
	}
}