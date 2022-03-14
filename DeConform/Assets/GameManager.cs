using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameStates
    {
        menu,
        playing,
        end
	}

    [SerializeField] private SmoothFollow sF;
    [SerializeField] private ColorUpdate cU;
    [SerializeField] private PlayerControls pC;
    [SerializeField] private AnimationController aC;
    

    [SerializeField] private RectTransform playerStartPos;

    public float playTime;

    public Animator animator;

    public GameStates gameState;

    private bool endAnimIsDone = false;

    void Start()
    {
        gameState = GameStates.menu;

        //Set player position
        pC.transform.position = new Vector3(playerStartPos.position.x, playerStartPos.position.y, 0);

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
                animator.SetTrigger("LeaveMenu");
                gameState = GameStates.playing;
            }
		}
        else if (gameState == GameStates.playing)
        {
            sF.enabled = true;
            cU.enabled = true;
            pC.enabled = true;
            aC.enabled = false;
            playTime += Time.deltaTime;
        }
        else if (gameState == GameStates.end)
        {
            sF.enabled = true;
            cU.enabled = false;
            pC.enabled = false;
            aC.enabled = false;
			if (endAnimIsDone && Input.GetMouseButtonDown(0))
			{
                SceneManager.LoadScene(0);
			}
        }
    }

    public void EndAnimFinished()
	{
        endAnimIsDone = true;
    }
}