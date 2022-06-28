using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CFcameraController : MonoBehaviour
{
	[SerializeField] private float menuSize;
	[SerializeField] private float playSize;
	[SerializeField] private AnimationCurve animateToSizeCurve;
	private CFsmoothFollow smoothFollow;
	private float animateToSizeDuration;
	private bool animateToSizeInit = true;
	private bool animateToSize = false;
	private float animateToSizeTimeValue = 0;
	private float oldSize;
	private float targetSize;

	private Camera cam;

	private void Start()
	{
		cam = GetComponent<Camera>();
		smoothFollow = GetComponent<CFsmoothFollow>();
		smoothFollow.following = false;
	}

	private void Update()
	{
		AnimToSize();
	}

	public void StartFollowingTarget(Transform target)
	{
		smoothFollow.targetTransform = target;
		smoothFollow.following = true;
	}

	public void StopFollowingTarget(float duration)
	{
		smoothFollow.StartAnimateToPerfectFollow(duration);
		smoothFollow.following = false;
		smoothFollow.targetTransform = null;
	}

	public void StartAnimateToPlaySize(float animationLength)
	{
		targetSize = playSize;
		animateToSizeDuration = animationLength;
		animateToSize = true;
		animateToSizeInit = true;
	}

	public void StartAnimateToMenuSize(float animationLength)
	{
		targetSize = menuSize;
		animateToSizeDuration = animationLength;
		animateToSize = true;
		animateToSizeInit = true;
	}

	private void AnimToSize()
	{
		if (!animateToSize) { return; }

		if (animateToSizeInit)
		{
			oldSize = cam.orthographicSize;
			animateToSizeTimeValue = 0;
			animateToSizeInit = false;
		}

		if (animateToSizeTimeValue > 0 && animateToSizeTimeValue <= animateToSizeDuration)
		{
			float EvaluatedTimeValueIn = animateToSizeCurve.Evaluate(animateToSizeTimeValue / animateToSizeDuration);

			float NewSize = Mathf.Lerp(oldSize, targetSize, EvaluatedTimeValueIn);
			cam.orthographicSize = NewSize;
		}
		else if (animateToSizeTimeValue > animateToSizeDuration)
		{
			animateToSizeInit = true;
			animateToSize = false;
		}

		animateToSizeTimeValue += Time.deltaTime;
		Mathf.Clamp(animateToSizeTimeValue, 0, animateToSizeDuration + 0.01f);
	}
}
