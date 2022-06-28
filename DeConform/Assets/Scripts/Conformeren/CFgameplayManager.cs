using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CFgameplayManager : MonoBehaviour
{
	[SerializeField] private int playIndex;
	[SerializeField] private bool useTimer;
	[SerializeField] private bool useStopWatch;
	[SerializeField] private float startTime;
	private float timerValue;
	[SerializeField] private CFmenuElementBehaviour menuElement;
	[SerializeField] private CFcameraController cam;
	[SerializeField] private CFsmoothFollow camFollow;
	[SerializeField] private CFplayerController player;
	[SerializeField] private CFcolorUpdater playerColor;
	[SerializeField] private Transform playerDirectionLookAt;
	[SerializeField] private CFbackgroundManager background;
	[SerializeField] private CFobstacleManager obstacle;
	[SerializeField] private Transform worldSpacePlayParent;
	[SerializeField] private TextMeshProUGUI timeDisplay;
	private Color timeDisplayColor;

	private BaseGameplay baseGameplay;

	private bool startUpSequenceStarted = false;
	private float startUpTimeValue = 0;
	[Header("Start Sequence")]
	[SerializeField] private float animateToCentreDelay;
	[SerializeField] private float animateToCentreDuration;
	[SerializeField] private float animateSmoothSpeedStart;
	[SerializeField] private AnimationCurve animateSmoothSpeedCurve;
	private float normalSmoothSpeed;
	private bool animateToCentreStarted = false;
	[Space(10)]
	[SerializeField] private float zoomCameraInDelay;
	[SerializeField] private float zoomCameraInDuration;
	private bool zoomCameraInStarted = false;
	[Space(10)]
	[SerializeField] private float fadeBackgroundInDelay;
	[SerializeField] private float fadeBackgroundInDuration;
	[SerializeField] private float backgroundPlayOpacity;
	[Space(10)]
	[SerializeField] private float animateInTimerDelay;
	[SerializeField] private float animateInTimerDuration;
	[SerializeField] private AnimationCurve animateInTimerCurve;
	private bool fadeBackgroundInStarted = false;
	private float startUpLength;
	private bool startUpSequenceFinished = false;

	private bool isPlaying = false;
	[Header("Playing Settings")]
	[SerializeField] private float graceTime;
	private float graceTimerValue = 0;
	private bool gracePeriodStarted;

	private bool closeDownSequenceStarted = false;
	private bool closeDownSequenceinit = true;
	private float closeDownTimeValue = 0;
	[Header("Close Sequence")]
	[SerializeField] private float centreGraceDuration;
	[SerializeField] private float rotationSmoothSpeed;
	private bool finishedCentreGrace = false;
	[SerializeField] private float zoomOutDelay;
	[SerializeField] private float zoomOutDuration;
	private bool startedZoomOut = false;
	private bool playerSwappedOutro = false;
	[Space(10)]
	[SerializeField] private float fadeBackgroundOutDelay;
	[SerializeField] private float fadeBackgroundOutDuration;
	[SerializeField] private float backgroundMenuOpacity;
	private bool startedBackgroundFadeOut = false;
	[Space(10)]
	[SerializeField] private float animateToTitleDelay;
	[SerializeField] private float animateToTitleDuration;
	private bool startedAnimateToTitle = false;
	[Space(10)]
	[SerializeField] private float animateOutTimerDelay;
	[SerializeField] private float animateOutTimerDuration;
	[SerializeField] private AnimationCurve animateOutTimerCurve;
	private float closeDownLength;
	private bool closeDownSequenceFinished = false;

	public void StartUp()
	{
		timeDisplayColor = timeDisplay.color;
		timeDisplay.color = new Color(timeDisplayColor.r, timeDisplayColor.g, timeDisplayColor.b, 0);
		timeDisplay.text = FormatTimer(startTime);
		timerValue = startTime;

		baseGameplay = GetComponent<BaseGameplay>();

		startUpLength = animateToCentreDuration + animateToCentreDelay;
		if(zoomCameraInDuration + zoomCameraInDelay > startUpLength) { startUpLength = zoomCameraInDuration + zoomCameraInDelay; }
		if(fadeBackgroundInDuration + fadeBackgroundInDelay > startUpLength) { startUpLength = fadeBackgroundInDuration + zoomCameraInDelay; }
		if(animateInTimerDuration + animateInTimerDelay > startUpLength) { startUpLength = animateInTimerDuration + animateInTimerDelay; }

		closeDownLength = zoomOutDelay + zoomOutDuration;
		if(fadeBackgroundOutDelay + fadeBackgroundOutDuration > closeDownLength) { closeDownLength = fadeBackgroundOutDelay + fadeBackgroundOutDuration; }
		if(animateToTitleDelay + animateToTitleDuration > closeDownLength) { closeDownLength = animateToTitleDelay + animateToTitleDuration; }
		closeDownLength += centreGraceDuration;
		if(animateOutTimerDuration + animateOutTimerDelay > closeDownLength) { closeDownLength = animateOutTimerDuration + animateOutTimerDelay; }

		worldSpacePlayParent.gameObject.SetActive(true);

		startUpSequenceStarted = true;
	}

	private void Update()
	{
		StartUpSequence();
		WhilePlaying();
		CloseDownSequence();
	}

	private void StartUpSequence()
	{
		if (startUpSequenceFinished || !startUpSequenceStarted) { return; }

		//Animate menu element to center
		if(startUpTimeValue > animateToCentreDelay && !animateToCentreStarted)
		{
			normalSmoothSpeed = camFollow.positionSmoothSpeed;
			camFollow.positionSmoothSpeed = animateSmoothSpeedStart;
			//player.transform.localPosition = menuElement.transform.localPosition;
			menuElement.SetOpacity(0);
			player.transform.SetParent(worldSpacePlayParent);
			player.gameObject.SetActive(true);
			cam.StartFollowingTarget(player.transform);
			background.StartFollowingTarget(player.transform, true);
			obstacle.transform.position = player.transform.position;
			//menuElement.StartAnimateToCentre(animateToCentreDuration);
			animateToCentreStarted = true;
		}
		else if(startUpTimeValue > animateToCentreDelay && startUpTimeValue <= animateToCentreDuration)
		{
			float EvaluatedTimeValue = animateSmoothSpeedCurve.Evaluate((startUpTimeValue - animateToCentreDelay) / animateToCentreDuration);

			float newValue = Mathf.Lerp(animateSmoothSpeedStart, normalSmoothSpeed, EvaluatedTimeValue);
			camFollow.positionSmoothSpeed = newValue;
		}

		//zoom camera in
		if (startUpTimeValue > zoomCameraInDelay && !zoomCameraInStarted)
		{
			cam.StartAnimateToPlaySize(zoomCameraInDuration);
			zoomCameraInStarted = true;
		}
		//highten BG opacity
		if (startUpTimeValue > fadeBackgroundInDelay && !fadeBackgroundInStarted)
		{
			background.StartOpacityAnim(new Vector2(fadeBackgroundInDuration, backgroundPlayOpacity));
			fadeBackgroundInStarted = true;
		}
		//fade in timer
		if(startUpTimeValue > animateInTimerDelay && startUpTimeValue <= animateInTimerDelay + animateInTimerDuration)
		{
			float EvaluatedTimeValue = animateInTimerCurve.Evaluate((startUpTimeValue - animateInTimerDelay) / animateInTimerDuration);

			Color newColor = Color.Lerp(new Color(timeDisplayColor.r, timeDisplayColor.g, timeDisplayColor.b, 0), timeDisplayColor, EvaluatedTimeValue);
			timeDisplay.color = newColor;
		}

		if(startUpTimeValue > startUpLength)
		{
			//Swap out menu element for the real one, parent real one to ObjectsA, Enable all player scripts, Enable Timer
			camFollow.positionSmoothSpeed = normalSmoothSpeed;
			//Enable collider spawns
			obstacle.StartUp();
			player.enabled = true;
			isPlaying = true;
			startUpSequenceFinished = true;
		}

		startUpTimeValue += Time.deltaTime;
	}

	private void WhilePlaying()
	{
		if (!isPlaying) { return; }

		//Timer stuffz
		if (useTimer && player.playerHasTouched)
		{
			timeDisplay.text = FormatTimer(timerValue);

			if(timerValue > 0.5f)
			{
				timerValue -= Time.deltaTime;
			}
			else
			{
				//end
				closeDownSequenceStarted = true;
				closeDownSequenceinit = true;
				isPlaying = false;
			}
		}

		if (useStopWatch && player.playerHasTouched)
		{
			timerValue += Time.deltaTime;
			timeDisplay.text = FormatTimer(timerValue);
		}

		//Check for end
		if(playerColor.valueProgress == 1 || playerColor.valueProgress == 0)
		{
			gracePeriodStarted = true;
		}
		else
		{
			gracePeriodStarted = false;
			graceTimerValue = 0;
		}

		if (gracePeriodStarted)
		{
			if(graceTimerValue > graceTime)
			{
				//end
				closeDownSequenceStarted = true;
				closeDownSequenceinit = true;
				isPlaying = false;
			}

			graceTimerValue += Time.deltaTime;
		}
	}

	private void CloseDownSequence()
	{
		if(!closeDownSequenceStarted || closeDownSequenceFinished) { return; }

		if (closeDownSequenceinit)
		{
			player.enabled = false;
			for (int i = 0; i < obstacle.obstacles.Count; i++)
			{
				obstacle.obstacles[i].SetActive(false);
			}
			player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			playerColor.enabled = false;
			playerColor.targetRenderer.GetComponent<CFsmoothFollow>().rotationSmoothSpeed = rotationSmoothSpeed;
			playerDirectionLookAt.rotation = Quaternion.Euler(0, 0, 45);
			if(playIndex == 0)
			{
				CFmenuElementManager.instance.ownDiamondOne = playerColor.targetRenderer.color;
				//Communicate results to Datamanager
				DataManager.instance.currentSaveData.gameResult += $"{ColorUtility.ToHtmlStringRGB(playerColor.targetRenderer.color)}k";
			}
			else if(playIndex == 1)
			{
				CFmenuElementManager.instance.ownDiamondTwo = playerColor.targetRenderer.color;
				//Communicate results to Datamanager
				DataManager.instance.currentSaveData.gameResult += $"{ColorUtility.ToHtmlStringRGB(playerColor.targetRenderer.color)}k";
			}
			else if(playIndex == 2)
			{
				CFmenuElementManager.instance.ownDiamondThree = playerColor.targetRenderer.color;
				//Communicate results to Datamanager
				DataManager.instance.currentSaveData.gameResult += $"{ColorUtility.ToHtmlStringRGB(playerColor.targetRenderer.color)}";
				//Communicate stopwatch result
				DataManager.instance.currentSaveData.gameResult += $"t{timerValue}";
			}
			cam.StopFollowingTarget(centreGraceDuration);
			closeDownTimeValue = 0;
			closeDownSequenceinit = false;
		}

		if(closeDownTimeValue > centreGraceDuration && !finishedCentreGrace)
		{
			//Swap out real element with menu element
			camFollow.positionSmoothSpeed = normalSmoothSpeed;
			menuElement.transform.localPosition = Vector3.zero;
			menuElement.UpdateColor();
			
			background.StopFollowingTarget();
			finishedCentreGrace = true;
		}

		if(closeDownTimeValue > centreGraceDuration + animateToTitleDelay && !startedAnimateToTitle)
		{
			menuElement.StartAnimateToTitle(animateToTitleDuration);
			startedAnimateToTitle = true;
		}

		if(closeDownTimeValue > centreGraceDuration + fadeBackgroundOutDelay && !startedBackgroundFadeOut)
		{
			background.StartOpacityAnim(new Vector2(fadeBackgroundOutDuration, backgroundMenuOpacity));
			startedBackgroundFadeOut = true;
		}

		if(closeDownTimeValue > centreGraceDuration + zoomOutDelay && !startedZoomOut)
		{
			cam.StartAnimateToMenuSize(zoomOutDuration);
			startedZoomOut = true;
		}
		else if (closeDownTimeValue > centreGraceDuration + zoomOutDelay + zoomOutDuration && !playerSwappedOutro)
		{
			player.gameObject.SetActive(false);
			menuElement.SetOpacity(1);
			playerSwappedOutro = true;
		}
		//fade out timer
		if (closeDownTimeValue > animateOutTimerDelay && closeDownTimeValue <= animateOutTimerDelay + animateOutTimerDuration)
		{
			float EvaluatedTimeValue = animateOutTimerCurve.Evaluate((closeDownTimeValue - (animateOutTimerDelay)) / animateOutTimerDuration);

			Color newColor = Color.Lerp(timeDisplayColor, new Color(timeDisplayColor.r, timeDisplayColor.g, timeDisplayColor.b, 0), EvaluatedTimeValue);
			timeDisplay.color = newColor;
		}


		if (closeDownTimeValue > closeDownLength)
		{
			baseGameplay.StartCloseDown();
			closeDownSequenceFinished = true;
		}

		closeDownTimeValue += Time.deltaTime;
	}

	private string FormatTimer(float time)
	{
		int hours = Mathf.FloorToInt(time / 360);
		int minutes = Mathf.FloorToInt((time % 360) / 60);
		int seconds = Mathf.FloorToInt((time % 60));

		string output = "";
		if(hours > 0)
		{
			output += $"{hours}:";
		}

		if(minutes > 9)
		{
			output += $"{minutes}:";
		}
		else
		{
			output += $"0{minutes}:";
		}

		if (seconds > 9)
		{
			output += $"{seconds}";
		}
		else
		{
			output += $"0{seconds}";
		}

		return output;
	}
}
