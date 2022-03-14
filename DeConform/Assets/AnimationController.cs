using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
	[SerializeField] private Material backgroundMat;
	[SerializeField] private float scrollSpeed;
	private float yOffset = 0;

	private void Update()
	{
		yOffset += Time.deltaTime * scrollSpeed;
		backgroundMat.SetFloat("_NoiseOffsetY", yOffset);
	}
}
