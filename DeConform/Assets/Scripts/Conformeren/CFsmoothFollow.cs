using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFsmoothFollow : MonoBehaviour
{
    [SerializeField] public Transform targetTransform;
    [SerializeField] private bool followPosition;
    public float positionSmoothSpeed;
    [SerializeField] private bool followRotation;
    public float rotationSmoothSpeed;

    [SerializeField] private AnimationCurve perfectFollowCurve;

    public bool following;

    private Vector3 oldPosition;
    private Quaternion oldRot;
    private Vector3 targetPosition;
    private Quaternion targetRot;

    private float animateToTargetDuration;
    private bool animateToTargetInit = true;
    private bool animateToTarget = false;
    private float animateToTargetTimeValue = 0;

    //Animate positionSmoothSpeed;

    private void Start()
	{

	}

	// Update is called once per frame
	private void FixedUpdate()
    {
        if(targetTransform == null || !following || animateToTarget) { return; }
		if (followPosition)
		{
            Vector3 curPos = transform.position;
            Vector3 targetPos = new Vector3(targetTransform.position.x, targetTransform.position.y, curPos.z);
            Vector3 smoothedPosition = Vector3.Lerp(curPos, targetPos, positionSmoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }

		if (followRotation)
		{
            Quaternion curRot = transform.rotation;
            Quaternion targetRot = targetTransform.rotation;
            Quaternion smoothedRotation = Quaternion.Lerp(curRot, targetRot, rotationSmoothSpeed * Time.deltaTime);
            transform.rotation = smoothedRotation;
		}
    }

	private void Update()
	{
        AnimToTarget();
    }

	public void StartAnimateToPerfectFollow(float duration)
	{
        if (followPosition) { targetPosition = targetTransform.position; }
        if (followRotation) { targetRot = targetTransform.rotation; }
        animateToTargetDuration = duration;
        animateToTargetInit = true;
        animateToTarget = true;
    }

    private void AnimToTarget()
	{
		if (!animateToTarget) { return; }

		if (animateToTargetInit)
		{
            oldPosition = transform.position;
            animateToTargetTimeValue = 0;
            animateToTargetInit = false;
		}

        if(animateToTargetTimeValue > 0 && animateToTargetTimeValue <= animateToTargetDuration)
		{
            float EvaluatedTime = perfectFollowCurve.Evaluate(animateToTargetTimeValue / animateToTargetDuration);

			if (followPosition)
			{
                Vector3 NewPos = Vector3.Lerp(oldPosition, new Vector3(targetPosition.x, targetPosition.y, oldPosition.z), EvaluatedTime);
                transform.position = NewPos;
            }
			if (followRotation)
			{
                Quaternion NewRot = Quaternion.Lerp(oldRot, targetRot, EvaluatedTime);
                transform.rotation = NewRot;
			}
        }
        else if(animateToTargetTimeValue > animateToTargetDuration)
		{
            animateToTargetInit = true;
            animateToTarget = false;
        }

        animateToTargetTimeValue += Time.deltaTime;
        Mathf.Clamp(animateToTargetTimeValue, 0, animateToTargetDuration + 0.01f);
    }


}
