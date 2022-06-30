using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CFdefinitionResultHandler : MonoBehaviour
{
	private HighlightColorManager hCM;
	private BaseDefinition baseDef;
	private DataManager dataManager;
	private string resultTime;
	private string resultColors;
	[SerializeField] private bool isFirst;
	[SerializeField] private bool isBaseDef;
	[SerializeField] private bool isInput;
	[SerializeField] private bool isOwnDef;

	[Header("References")]
	[SerializeField] private TextMeshProUGUI definitionText;
	[SerializeField] private SpriteRenderer elementOne;
	[SerializeField] private SpriteRenderer elementTwo;
	[SerializeField] private SpriteRenderer elementThree;
	[SerializeField] private TextMeshProUGUI timeText;
	[SerializeField] private float timeTextOpacity;
	[SerializeField] private RectTransform resultPivotTransform;

	[Header("Animation Settings")]
	[SerializeField] private AnimationCurve introCurve;
	[SerializeField] private AnimationCurve outroCurve;
	[SerializeField] private float elementOneDelay;
	[SerializeField] private float elementTwoDelay;
	[SerializeField] private float elementThreeDelay;
	private float introDuration;
	private float outroDuration;
	[Space(20)]
	[SerializeField] private float firstLineHeightInPixels;
	[SerializeField] private float normalLineHeightInPixels;
	[SerializeField] private float firsthalfLineHeightInPixels;

	private bool animateIn = false;
	private bool animateInInit = true;
	private float animateInTimeValue = 0;

	private bool animateOut = false;
	private bool animateOutInit = true;
	private float animateOutTimeValue = 0;

	public void StartUp(float Duration)
	{
		introDuration = Duration;
		hCM = HighlightColorManager.instance;
		dataManager = DataManager.instance;
		string serverString = "";
		if (isBaseDef)
		{
			baseDef = GetComponent<BaseDefinition>();
			serverString = baseDef.results;
		}
		else if (isInput || isOwnDef)
		{
			serverString = dataManager.currentSaveData.gameResult;
		}

		timeText.enabled = false;
		elementOne.enabled = false;
		elementTwo.enabled = false;
		elementThree.enabled = false;
		timeText.color = new Color(timeText.color.r, timeText.color.g, timeText.color.b, 0);
		elementOne.color = new Color(0, 0, 0, 0);
		elementTwo.color = new Color(0, 0, 0, 0);
		elementThree.color = new Color(0, 0, 0, 0);

		if (!serverString.Contains('k') || !serverString.Contains('t')) { Debug.Log("result string is invalid"); return; }
		resultColors = serverString.Split('t')[0];
		resultTime = serverString.Split('t')[1];

		//Set all elements to the correct color, but with transperancy zero
		string[] hexCodes = resultColors.Split('k');
		Color colOne;
		if(ColorUtility.TryParseHtmlString($"#{hexCodes[0]}", out colOne))
		{
			elementOne.color = new Color(colOne.r, colOne.g, colOne.b, 0);
		}
		Color colTwo;
		if (ColorUtility.TryParseHtmlString($"#{hexCodes[1]}", out colTwo))
		{
			elementTwo.color = new Color(colTwo.r, colTwo.g, colTwo.b, 0);
		}
		Color colThree;
		if (ColorUtility.TryParseHtmlString($"#{hexCodes[2]}", out colThree))
		{
			elementThree.color = new Color(colThree.r, colThree.g, colThree.b, 0);
		}

		//Set timer to correct time, but with transperancy zero
		timeText.text = FormatTimer(float.Parse(resultTime));
		//calc pivot position
		definitionText.ForceMeshUpdate();
		float yTopDef = definitionText.GetComponent<RectTransform>().rect.max.y + definitionText.GetComponent<RectTransform>().anchoredPosition.y; Debug.LogWarning($"yTopDef = {yTopDef}");
		resultPivotTransform.anchoredPosition = new Vector2(resultPivotTransform.anchoredPosition.x, yTopDef - firstLineHeightInPixels - (definitionText.textInfo.lineCount - 2) * normalLineHeightInPixels - firsthalfLineHeightInPixels - resultPivotTransform.rect.height / 2);
		Debug.LogWarning($"yPosScoreText = {yTopDef} - {firstLineHeightInPixels} - {definitionText.textInfo.lineCount} * {normalLineHeightInPixels} - {firsthalfLineHeightInPixels} - {resultPivotTransform.rect.height / 2}");

		animateIn = true;
	}

	public void CloseDown(float Duration)
	{
		outroDuration = Duration;
		animateOut = true;
	}

	private void Update()
	{
		AnimIn();
		AnimOut();
	}

	private void AnimIn()
	{
		if(!animateIn) { return; }

		if (animateInInit)
		{
			timeText.enabled = true;
			elementOne.enabled = true;
			elementTwo.enabled = true;
			elementThree.enabled = true;
			animateInTimeValue = 0;
			animateInInit = false;
		}

		if(animateInTimeValue > elementOneDelay && animateInTimeValue <= introDuration)
		{
			float EvaluatedTime = introCurve.Evaluate((animateInTimeValue - elementOneDelay) / (introDuration - elementOneDelay));

			elementOne.color = new Color(elementOne.color.r, elementOne.color.g, elementOne.color.b, EvaluatedTime);
		}

		if (animateInTimeValue > elementTwoDelay && animateInTimeValue <= introDuration)
		{
			float EvaluatedTime = introCurve.Evaluate((animateInTimeValue - elementTwoDelay) / (introDuration - elementTwoDelay));

			elementTwo.color = new Color(elementTwo.color.r, elementTwo.color.g, elementTwo.color.b, EvaluatedTime);
		}

		if (animateInTimeValue > elementThreeDelay && animateInTimeValue <= introDuration)
		{
			float EvaluatedTime = introCurve.Evaluate((animateInTimeValue - elementThreeDelay) / (introDuration - elementThreeDelay));

			elementThree.color = new Color(elementThree.color.r, elementThree.color.g, elementThree.color.b, EvaluatedTime);
		}

		if (animateInTimeValue > 0 && animateInTimeValue <= introDuration)
		{
			float EvaluatedTime = introCurve.Evaluate(animateInTimeValue / introDuration);

			timeText.color = new Color(timeText.color.r, timeText.color.g, timeText.color.b, EvaluatedTime * timeTextOpacity);
		}
		else if(animateInTimeValue > introDuration)
		{
			animateInInit = true;
			animateIn = false;
		}

		animateInTimeValue += Time.deltaTime;
		Mathf.Clamp(animateInTimeValue, 0, introDuration + 0.01f);
	}

	private void AnimOut()
	{
		if (!animateOut) { return; }

		if (animateOutInit)
		{
			animateOutTimeValue = 0;
			animateOutInit = false;
		}

		if (animateOutTimeValue > 0 && animateOutTimeValue <= outroDuration)
		{
			float EvaluatedTime = outroCurve.Evaluate(animateOutTimeValue / outroDuration);

			timeText.color = new Color(timeText.color.r, timeText.color.g, timeText.color.b, (1 - EvaluatedTime) * timeTextOpacity);
			elementOne.color = new Color(elementOne.color.r, elementOne.color.g, elementOne.color.b, 1 - EvaluatedTime);
			elementTwo.color = new Color(elementTwo.color.r, elementTwo.color.g, elementTwo.color.b, 1 - EvaluatedTime);
			elementThree.color = new Color(elementThree.color.r, elementThree.color.g, elementThree.color.b, 1 - EvaluatedTime);
		}
		else if (animateOutTimeValue > outroDuration)
		{
			animateOutInit = true;
			animateOut = false;
		}

		animateOutTimeValue += Time.deltaTime;
		Mathf.Clamp(animateOutTimeValue, 0, outroDuration + 0.01f);
	}

	private string FormatTimer(float time)
	{
		int hours = Mathf.FloorToInt(time / 360);
		int minutes = Mathf.FloorToInt((time % 360) / 60);
		int seconds = Mathf.FloorToInt((time % 60));

		string output = "";
		if (hours > 0)
		{
			output += $"{hours}:";
		}

		if (minutes > 9)
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
