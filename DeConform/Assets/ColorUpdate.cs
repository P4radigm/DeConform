using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ColorUpdate : MonoBehaviour
{
    [SerializeField] private Camera renderTexCam;
    [SerializeField] private RenderTexture renderTex;
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private float changeSpeed;
    [Space(20)]
    [SerializeField] private Image floodPanel;
    [SerializeField] private TextMeshProUGUI[] endTexts;
    [SerializeField] private TextMeshProUGUI timer;
    private float currentMiddlePixelValue;
    //private Color startingCol;
    private Vector3 startHSVcol;
    private Vector3 updateHSVcol;
    private Texture2D tex;

    [SerializeField] private GameManager gM;

    // Start is called before the first frame update
    void Start()
    {
        Color.RGBToHSV(targetRenderer.color, out startHSVcol.x, out startHSVcol.y, out startHSVcol.z);
        updateHSVcol = startHSVcol;

        tex = new Texture2D(16, 16, TextureFormat.RGB24, false);
    }

    // Update is called once per frame
    void Update()
    {
        //Get middle of screen pixel value
        renderTexCam.targetTexture = renderTex;
        renderTexCam.Render();
        RenderTexture.active = renderTex;

        tex.ReadPixels(new Rect(8, 8, 2, 2), 8, 8);
        tex.Apply();
        currentMiddlePixelValue = tex.GetPixel(8, 8).r;

        //update player visuals
        if(currentMiddlePixelValue == 1)
		{
            if (updateHSVcol.z < 0.99)
            {
                updateHSVcol.z += changeSpeed * Time.deltaTime;
            }
			else
			{
                updateHSVcol.y -= changeSpeed * Time.deltaTime;
            }
        }
		else if(currentMiddlePixelValue == 0)
		{
            if (updateHSVcol.y < 0.99)
            {
                updateHSVcol.y += changeSpeed * Time.deltaTime;
            }
            else
            {
                updateHSVcol.z -= changeSpeed * Time.deltaTime;
            }

        }

        if(updateHSVcol.z <= 0.01f)
		{
            Debug.Log("You chose black");
            floodPanel.color = new Color(0, 0, 0, 0);
			for (int i = 0; i < endTexts.Length; i++)
			{
                endTexts[i].color = new Color(1, 1, 1, 0);
            }

            timer.text = TimeFormat(gM.playTime);

            gM.animator.SetTrigger("LeavePlaying");
            gM.gameState = GameManager.GameStates.end;
        }

        if(updateHSVcol.y <= 0.01f)
		{
            Debug.Log("You chose white");
            floodPanel.color = new Color(1, 1, 1, 0);
            for (int i = 0; i < endTexts.Length; i++)
            {
                endTexts[i].color = new Color(0, 0, 0, 0);
            }

            timer.text = TimeFormat(gM.playTime);

            gM.animator.SetTrigger("LeavePlaying");
            gM.gameState = GameManager.GameStates.end;
        }

        targetRenderer.color = Color.HSVToRGB(updateHSVcol.x, updateHSVcol.y, updateHSVcol.z);
    }

    string TimeFormat(float timeInSeconds)
	{
        int minutes = Mathf.FloorToInt((timeInSeconds / 60));
        int seconds = Mathf.FloorToInt((timeInSeconds % 60));
        int miliseconds = Mathf.FloorToInt((timeInSeconds * 1000) % 1000);

        string min = minutes.ToString();
        string sec = seconds.ToString();
        string mil = miliseconds.ToString();

        if(minutes < 10)
		{
            min = "0" + min;
		}
        if(seconds < 10)
		{
            sec = "0" + sec;
		}

        return min + ":" + sec + "." + mil;
	}
}
