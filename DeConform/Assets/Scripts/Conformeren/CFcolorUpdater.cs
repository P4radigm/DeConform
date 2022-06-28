using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFcolorUpdater : MonoBehaviour
{
    [SerializeField] private Camera renderTexCam;
    [SerializeField] private RenderTexture renderTex;
    [SerializeField] public SpriteRenderer targetRenderer;
    [SerializeField] private float changeSpeed;
    private Color baseColor;
    private float currentMiddlePixelValue;
    private Vector3 startHSVcol;
    [HideInInspector] public Vector3 updateHSVcol;
    private Texture2D tex;

    public float valueProgress = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        baseColor = targetRenderer.color;
        Color.RGBToHSV(targetRenderer.color, out startHSVcol.x, out startHSVcol.y, out startHSVcol.z);
        updateHSVcol = startHSVcol;

        tex = new Texture2D(1, 1, TextureFormat.RGB24, false);
    }

    // Update is called once per frame
    void Update()
    {
        //Get middle of screen pixel value
        renderTexCam.targetTexture = renderTex;
        renderTexCam.Render();
        RenderTexture.active = renderTex;

        tex.ReadPixels(new Rect(0, 0, 1, 1), 0, 0);
        tex.Apply();
        currentMiddlePixelValue = tex.GetPixel(0, 0).r;

        //update player visuals
        if (currentMiddlePixelValue == 1)
        {
            if (updateHSVcol.z < 1)
            {
                updateHSVcol.z += changeSpeed * Time.deltaTime;
            }
            else if (updateHSVcol.y > 0)
            {
                updateHSVcol.y -= changeSpeed * Time.deltaTime;
            }
        }
        else if (currentMiddlePixelValue == 0)
        {
            if (updateHSVcol.y < 1)
            {
                updateHSVcol.y += changeSpeed * Time.deltaTime;
            }
            else if (updateHSVcol.z > 0)
            {
                updateHSVcol.z -= changeSpeed * Time.deltaTime;
            }
        }

        //Clamp S & V Values
        updateHSVcol.y = Mathf.Clamp(updateHSVcol.y, 0, 1);
        updateHSVcol.z = Mathf.Clamp(updateHSVcol.z, 0, 1);

        targetRenderer.color = Color.HSVToRGB(updateHSVcol.x, updateHSVcol.y, updateHSVcol.z);

        valueProgress = 1 - ((1- updateHSVcol.z) * 0.5f + updateHSVcol.y * 0.5f);
    }
}
