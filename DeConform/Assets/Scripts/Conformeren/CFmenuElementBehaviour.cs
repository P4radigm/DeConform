using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CFmenuElementBehaviour : MonoBehaviour
{
	private CFmenuElementManager mEM;
	private HighlightColorManager hCM;
	private SpriteRenderer sP;
	[SerializeField] private int position;
	[SerializeField] private bool isConformeren;
	[SerializeField] private bool startTransparant;

	[SerializeField] private AnimationCurve animateFadeInCurve;
	private bool animateFadeInInit = true;
	private bool animateFadeIn = false;
	private float animateFadeInTimeValue = 0;
	private float animateFadeInDuration;
	private float oldOpacity;
	[SerializeField] private AnimationCurve animateFadeOutCurve;
	private bool animateFadeOutInit = true;
	private bool animateFadeOut = false;
	private float animateFadeOutTimeValue = 0;
	private float animateFadeOutDuration;

	[SerializeField] private AnimationCurve animateToPositionCurve;
	private bool animateToPositionInit = true;
	private bool animateToPosition = false;
	private float animateToPositionTimeValue = 0;
	private float animateToPositionDuration;
	private Vector3 oldLocalPosition;
	private Vector3 targetLocalPosition;

	private Vector3 titlePosition;
	private Vector3 centerPosition;

	private void OnEnable()
	{
		mEM = CFmenuElementManager.instance;
		hCM = HighlightColorManager.instance;
		sP = GetComponent<SpriteRenderer>();

		if (isConformeren)
		{
			if (position == 0)
			{
				sP.color = new Color(mEM.ownDiamondOne.r, mEM.ownDiamondOne.g, mEM.ownDiamondOne.b, startTransparant ? 0 : 1);
				titlePosition = mEM.localTransformPositionOne;
			}
			else if (position == 1)
			{
				sP.color = new Color(mEM.ownDiamondTwo.r, mEM.ownDiamondTwo.g, mEM.ownDiamondTwo.b, startTransparant ? 0 : 1);
				titlePosition = mEM.localTransformPositionTwo;
			}
			else if (position == 2)
			{
				sP.color = new Color(mEM.ownDiamondThree.r, mEM.ownDiamondThree.g, mEM.ownDiamondThree.b, startTransparant ? 0 : 1);
				titlePosition = mEM.localTransformPositionThree;
			}
			transform.localPosition = titlePosition;
		}
		else
		{
			Color col = hCM.getHighlightColor();
			sP.color = new Color(col.r, col.g, col.b, 0);
		}

		centerPosition = mEM.localTransformCentre;
	}

	private void Update()
	{
		AnimFadeIn();
		AnimFadeOut();

		AnimToPosition();
	}

	public void StartAnimateIn(float animationLength)
	{
		if (animateFadeOut)
		{
			animateFadeOut = false;
			animateFadeOutInit = true;
		}

		animateFadeInDuration = animationLength;

		animateFadeOutInit = true;
		animateFadeIn = true;
	}

	public void StartAnimateOut(float animationLength)
	{
		if (animateFadeIn)
		{
			animateFadeIn = false;
			animateFadeInInit = true;
		}

		animateFadeOutDuration = animationLength;

		animateFadeOutInit = true;
		animateFadeOut = true;
	}

	public void StartAnimateToCentre(float animationLength)
	{
		targetLocalPosition = centerPosition;
		animateToPositionDuration = animationLength;
		animateToPosition = true;
		animateToPositionInit = true;
	}

	public void StartAnimateToTitle(float animationLength)
	{
		targetLocalPosition = titlePosition;
		animateToPositionDuration = animationLength;
		animateToPosition = true;
		animateToPositionInit = true;
	}

	public void UpdateColor()
	{
		if (position == 0)
		{
			sP.color = new Color(mEM.ownDiamondOne.r, mEM.ownDiamondOne.g, mEM.ownDiamondOne.b, sP.color.a);
		}
		else if (position == 1)
		{
			sP.color = new Color(mEM.ownDiamondTwo.r, mEM.ownDiamondTwo.g, mEM.ownDiamondTwo.b, sP.color.a);
		}
		else if (position == 2)
		{
			sP.color = new Color(mEM.ownDiamondThree.r, mEM.ownDiamondThree.g, mEM.ownDiamondThree.b, sP.color.a);
		}
	}

	public void SetOpacity(float opacity)
	{
		sP.color = new Color(sP.color.r, sP.color.g, sP.color.b, opacity);
	}

	private void AnimFadeIn()
	{
		if (!animateFadeIn) { return; }

		if (animateFadeInInit)
		{
			oldOpacity = sP.color.a;
			animateFadeInTimeValue = 0;
			animateFadeInInit = false;
		}

		if (animateFadeInTimeValue > 0 && animateFadeInTimeValue <= animateFadeInDuration)
		{
			float EvaluatedTimeValueIn = animateFadeInCurve.Evaluate(animateFadeInTimeValue / animateFadeInDuration);

			float NewOpacity = Mathf.Lerp(oldOpacity, 1, EvaluatedTimeValueIn);
			sP.color = new Color(sP.color.r, sP.color.g, sP.color.b, NewOpacity);
		}
		else if (animateFadeInTimeValue > animateFadeInDuration)
		{
			animateFadeInInit = true;
			animateFadeIn = false;
		}

		animateFadeInTimeValue += Time.deltaTime;
		Mathf.Clamp(animateFadeInTimeValue, 0, animateFadeInDuration + 0.01f);
	}

	private void AnimFadeOut()
	{
		if (!animateFadeOut) { return; }

		if (animateFadeOutInit)
		{
			oldOpacity = sP.color.a;
			animateFadeOutTimeValue = 0;
			animateFadeOutInit = false;
		}

		if (animateFadeOutTimeValue > 0 && animateFadeOutTimeValue <= animateFadeOutDuration)
		{
			float EvaluatedTimeValueIn = animateFadeInCurve.Evaluate(animateFadeOutTimeValue / animateFadeOutDuration);

			float NewOpacity = Mathf.Lerp(oldOpacity, 0, EvaluatedTimeValueIn);
			sP.color = new Color(sP.color.r, sP.color.g, sP.color.b, NewOpacity);
		}
		else if (animateFadeOutTimeValue > animateFadeOutDuration)
		{
			animateFadeOutInit = true;
			animateFadeOut = false;
		}

		animateFadeOutTimeValue += Time.deltaTime;
		Mathf.Clamp(animateFadeOutTimeValue, 0, animateFadeOutDuration + 0.01f);
	}

	private void AnimToPosition()
	{
		if (!animateToPosition) { return; }

		if (animateToPositionInit)
		{
			oldLocalPosition = transform.localPosition;
			animateToPositionTimeValue = 0;
			animateToPositionInit = false;
		}

		if (animateToPositionTimeValue > 0 && animateToPositionTimeValue <= animateToPositionDuration)
		{
			float EvaluatedTimeValueIn = animateFadeInCurve.Evaluate(animateToPositionTimeValue / animateToPositionDuration);

			Vector3 NewPosition = Vector3.Lerp(oldLocalPosition, targetLocalPosition, EvaluatedTimeValueIn);
			transform.localPosition = NewPosition;
		}
		else if (animateToPositionTimeValue > animateToPositionDuration)
		{
			animateToPositionInit = true;
			animateToPosition = false;
		}

		animateToPositionTimeValue += Time.deltaTime;
		Mathf.Clamp(animateToPositionTimeValue, 0, animateToPositionDuration + 0.01f);
	}
}
