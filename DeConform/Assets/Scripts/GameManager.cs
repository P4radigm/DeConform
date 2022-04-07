using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public enum GameStates
    {
        menu,
        playing,
        definitionA,
        playingA,
        definitionB,
        playingB,
        end
	}

    [SerializeField] private SmoothFollow sF;
    [SerializeField] private ColorUpdate cU;
    [SerializeField] private PlayerControls pC;
    [SerializeField] private AnimationController aC;
    [SerializeField] private CameraZoom cZ;
    [SerializeField] private TouchMessageAnimations tMA;
    
    [SerializeField] private RectTransform playerStartPos;

    [SerializeField] private float firstTimer;
    [SerializeField] private float secondTimer;
    [SerializeField] private TextMeshProUGUI fgTimer;
    [SerializeField] private TextMeshProUGUI bgTimer;
    public float playTime;

    public Animator animator;
    public GameStates gameState;

    private bool endAnimIsDone = false;

    void Start()
    {
        gameState = GameStates.menu;

        //Set player position
        pC.transform.position = new Vector3(playerStartPos.position.x, playerStartPos.position.y, -1);

        tMA.StartBlinkLoop();

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Escape))
		{
            Application.Quit();
		}

        if(gameState == GameStates.menu)
		{
            sF.enabled = false;
            cU.enabled = false;
            pC.enabled = false;
            aC.enabled = true;


            if (Input.GetMouseButtonDown(0))
			{
                animator.SetTrigger("Play");
            }
		}
        else if (gameState == GameStates.playing)
        {
            sF.enabled = true;
            cU.enabled = true;
            pC.enabled = true;
            aC.enabled = false;
            playTime += Time.deltaTime;
            fgTimer.text = TimerFormat(firstTimer - playTime);
            bgTimer.text = TimerFormat(firstTimer - playTime);
            
            if(cU.targetRenderer.color == Color.black || cU.targetRenderer.color == Color.white || firstTimer - playTime <= 0)
            {
                animator.SetTrigger("Definition");
                gameState = GameStates.definitionA;
            }
            
        }
        else if (gameState == GameStates.definitionA)
        {
            //Put in new definitionA

            sF.enabled = false;
            cU.enabled = false;
            pC.enabled = false;
            aC.enabled = true;


            if (!tMA.addDelay && Input.GetMouseButtonDown(0))
            {
                animator.SetTrigger("Play");
                playTime = 0;
            }
        }
        else if (gameState == GameStates.playingA)
        {
            sF.enabled = true;
            cU.enabled = true;
            pC.enabled = true;
            aC.enabled = false;
            playTime += Time.deltaTime;
            fgTimer.text = TimerFormat(secondTimer - playTime);
            bgTimer.text = TimerFormat(secondTimer - playTime);

            if (cU.targetRenderer.color == Color.black || cU.targetRenderer.color == Color.white || secondTimer - playTime <= 0)
            {
                animator.SetTrigger("Definition");
                gameState = GameStates.definitionB;
            }
        }
        else if (gameState == GameStates.definitionB)
        {
            //Put in new definitionB

            sF.enabled = false;
            cU.enabled = false;
            pC.enabled = false;
            aC.enabled = true;


            if (!tMA.addDelay && Input.GetMouseButtonDown(0))
            {
                animator.SetTrigger("Play");
                playTime = 0;
            }
        }
        else if (gameState == GameStates.playingB)
        {
            sF.enabled = true;
            cU.enabled = true;
            pC.enabled = true;
            aC.enabled = false;
            playTime += Time.deltaTime;
            fgTimer.text = StopwatchFormat(playTime);
            bgTimer.text = StopwatchFormat(playTime);

            if (cU.targetRenderer.color == Color.black || cU.targetRenderer.color == Color.white)
            {
                animator.SetTrigger("End");
                gameState = GameStates.end;
            }
        }
        else if (gameState == GameStates.end)
        {
            sF.enabled = true;
            cU.enabled = false;
            pC.enabled = false;
            aC.enabled = false;

            //fix dat ze zelf een definitie in moeten vullen

			//if (endAnimIsDone && Input.GetMouseButtonDown(0))
			//{
   //             SceneManager.LoadScene(0);
			//}
        }
    }

    public void EndAnimFinished()
	{
        endAnimIsDone = true;
    }

    public void AnimateCameraIn()
	{
        cZ.StartZoomIn();
    }

    public void AnimateCameraOut()
    {
        cZ.StartZoomOut();
    }

    public void AddToGamestate(int addition)
	{
        int _NewState = (int)gameState + addition; 
        gameState = (GameStates)_NewState;
    }

    string TimerFormat(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt((timeInSeconds / 60));
        int seconds = Mathf.CeilToInt((timeInSeconds % 60));
        Debug.Log($"minutes = {minutes}, secondes = {seconds}");
        string min = minutes.ToString();
        string sec = seconds.ToString();

        if (minutes < 10)
        {
            min = "0" + min;
        }
        if (seconds < 10)
        {
            sec = "0" + sec;
        }

        return $"{min}m<color=#FF00D1>{sec}s</color>";
    }
    string StopwatchFormat(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt((timeInSeconds / 60));
        int seconds = Mathf.FloorToInt((timeInSeconds % 60));
        //int miliseconds = Mathf.FloorToInt((timeInSeconds * 1000) % 1000);
        Debug.Log($"minutes = {minutes}, secondes = {seconds}");
        string min = minutes.ToString();
        string sec = seconds.ToString();
        //string mil = miliseconds.ToString();

        if (minutes < 0)
        {
            minutes = 0;
        }
        if (seconds < 0)
        {
            sec = 0;
        }

        if (minutes < 10)
        {
            min = "0" + min;
        }
        if (seconds < 10)
        {
            sec = "0" + sec;
        }


        return $"{min}m<color=#FF00D1>{sec}s</color>";
    }
}
