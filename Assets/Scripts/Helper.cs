using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
	public static List<T> FindComponentsInChildWithTag<T> (this GameObject parent, string tag) where T:Component
	{
		Transform t = parent.transform;
		List<T> components = new List<T> ();
		foreach (Transform tr in t) {
			if (tr.tag == tag) {
				components.Add (tr.GetComponent<T> ());
			}
		}
		return components;
	}
}