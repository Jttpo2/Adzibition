using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtWork
{
	private GameObject canvasCube;

	public ArtWork (Transform artWorkSprite)
	{
		canvasCube = GameObject.CreatePrimitive (PrimitiveType.Cube);

	}
}