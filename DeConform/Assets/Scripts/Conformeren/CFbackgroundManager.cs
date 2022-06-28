using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFbackgroundManager : MonoBehaviour
{
    [SerializeField] private Material backgroundMat;
    private CFfollow follow;
    private CFsmoothFollow camerFollow;
    private Camera mainCamera;

    //Opacity animation
    [SerializeField] private AnimationCurve globalOpacityAnimationCurve;
    [SerializeField] private float menuOpacity;
    [SerializeField] private float playOpacity;
    private bool animateOpacityInit = true;
    private bool animateOpacity = false;
    private float animateOpacityTimeValue = 0;
    private float opacityTarget;
    private float oldOpacity;
    private float animateOpacityDuration;

    //Scroll animation
    [SerializeField] private AnimationCurve globalStartScrollAnimationCurve;
    [SerializeField] private float globalStartScrollDuration;
    private bool animateStartScrollInit = true;
    private bool animateStartScroll = false;
    private float animateStartScrollTimeValue = 0;

    [SerializeField] private AnimationCurve globalStopScrollAnimationCurve;
    [SerializeField] private float globalStopScrollDuration;
    private bool animateStopScrollInit = true;
    private bool animateStopScroll = false;
    private float animateStopScrollTimeValue = 0;
    [SerializeField] private float globalTargetScrollSpeed;
    private float oldScrollSpeed;
    private float currentScrollSpeed;
    

    void Start()
    {
        backgroundMat.SetFloat("_Opacity", 0f);
        backgroundMat.SetFloat("_AnimationOffsetX", Random.Range(-100f,100f));
        backgroundMat.SetFloat("_AnimationOffsetY", Random.Range(-100f,100f));

        mainCamera = Camera.main;
        follow = GetComponent<CFfollow>();
        camerFollow = mainCamera.GetComponent<CFsmoothFollow>();
        currentScrollSpeed = 0;
    }

    public void StartFollowingTarget(Transform newTarget, bool needsOffset)
	{
        if (needsOffset) { follow.accumulatedOffsetToPlayer += transform.position - newTarget.position; }
        follow.targetTransform = newTarget;
        follow.following = true;
    }

    public void StopFollowingTarget()
	{
        follow.following = false;
    }

    void Update()
    {
        AnimOpacity();

        AnimStartScroll();
        AnimScroll();
        AnimStopScroll();
    }

    public void StartOpacityAnim(Vector2 DurationAndOpacity)
	{
        opacityTarget = DurationAndOpacity.y;
        animateOpacity = true;
        animateOpacityDuration = DurationAndOpacity.x;
        animateOpacityInit = true;
	}

    public void StartOpacityAnimZero(float duration)
	{
        opacityTarget = 0;
        animateOpacity = true;
        animateOpacityDuration = duration;
        animateOpacityInit = true;
    }

    public void StartOpacityAnimMenu(float duration)
    {
        opacityTarget = menuOpacity;
        animateOpacity = true;
        animateOpacityDuration = duration;
        animateOpacityInit = true;
    }

    public void StartOpacityAnimPlay(float duration)
    {
        opacityTarget = playOpacity;
        animateOpacity = true;
        animateOpacityDuration = duration;
        animateOpacityInit = true;
    }

    private void AnimOpacity()
	{
        if(!animateOpacity) { return; }

		if (animateOpacityInit)
		{
            oldOpacity = backgroundMat.GetFloat("_Opacity");
            animateOpacityTimeValue = 0;
            animateOpacityInit = false;
        }

        if(animateOpacityTimeValue > 0 && animateOpacityTimeValue <= animateOpacityDuration)
		{
            float EvaluatedTimeValueIn = globalOpacityAnimationCurve.Evaluate(animateOpacityTimeValue / animateOpacityDuration);

            float NewOpacity = Mathf.Lerp(oldOpacity, opacityTarget, EvaluatedTimeValueIn);
            backgroundMat.SetFloat("_Opacity", NewOpacity);
        }
        else if(animateOpacityTimeValue > animateOpacityDuration)
		{
            animateOpacityInit = true;
            animateOpacity = false;
        }

        animateOpacityTimeValue += Time.deltaTime;
        Mathf.Clamp(animateOpacityTimeValue, 0, animateOpacityDuration + 0.01f);
    }

    public void StartScrollAnim()
	{
		if (animateStopScroll)
		{
            animateStopScroll = false;
            animateStopScrollInit = true;
        }

        animateStartScrollInit = true;
        animateStartScroll = true;
    }

    public void StopScrollAnim()
    {
        if (animateStartScroll)
        {
            animateStartScroll = false;
            animateStartScrollInit = true;
        }

        animateStopScrollInit = true;
        animateStopScroll = true;
    }

    private void AnimStartScroll()
	{
        if (!animateStartScroll) { return; }

        if (animateStartScrollInit)
        {
            oldScrollSpeed = currentScrollSpeed;
            animateStartScrollTimeValue = 0;
            animateStartScrollInit = false;
        }

        if (animateStartScrollTimeValue > 0 && animateStartScrollTimeValue <= globalStartScrollDuration)
        {
            float EvaluatedTimeValueIn = globalStartScrollAnimationCurve.Evaluate(animateStartScrollTimeValue / globalStartScrollDuration);

            float NewScrollSpeed = Mathf.Lerp(oldScrollSpeed, globalTargetScrollSpeed, EvaluatedTimeValueIn);
            currentScrollSpeed = NewScrollSpeed;
        }
        else if (animateStartScrollTimeValue > globalStartScrollDuration)
        {
            animateStartScrollInit = true;
            animateStartScroll = false;
        }

        animateStartScrollTimeValue += Time.deltaTime;
        Mathf.Clamp(animateStartScrollTimeValue, 0, globalStartScrollDuration + 0.01f);
    }

    private void AnimScroll()
	{
         float CurAnimationOffsetX = backgroundMat.GetFloat("_AnimationOffsetY");
         backgroundMat.SetFloat("_AnimationOffsetY", CurAnimationOffsetX += currentScrollSpeed * Time.deltaTime);
	}

    private void AnimStopScroll()
	{
        if (!animateStopScroll) { return; }

        if (animateStopScrollInit)
        {
            oldScrollSpeed = currentScrollSpeed;
            animateStopScrollTimeValue = 0;
            animateStopScrollInit = false;
        }

        if (animateStopScrollTimeValue > 0 && animateStopScrollTimeValue <= globalStopScrollDuration)
        {
            float EvaluatedTimeValueIn = globalStopScrollAnimationCurve.Evaluate(animateStopScrollTimeValue / globalStopScrollDuration);

            float NewScrollSpeed = Mathf.Lerp(oldScrollSpeed, 0, EvaluatedTimeValueIn);
            currentScrollSpeed = NewScrollSpeed;
        }
        else if (animateStopScrollTimeValue > globalStopScrollDuration)
        {
            animateStopScrollInit = true;
            animateStopScroll = false;
        }

        animateStopScrollTimeValue += Time.deltaTime;
        Mathf.Clamp(animateStopScrollTimeValue, 0, globalStopScrollDuration + 0.01f);
    }
}
