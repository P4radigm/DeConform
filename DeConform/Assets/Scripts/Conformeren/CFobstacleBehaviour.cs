using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFobstacleBehaviour : MonoBehaviour
{
    public CFobstacleManager manager;
    private SpringJoint2D springJoint;
    private CircleCollider2D col;
    public bool isWhite;
    public bool isSetUp = false;

    public void SetUp(Vector3 cameraPosition)
    {
        springJoint = GetComponentInChildren<SpringJoint2D>();
        col = GetComponentInChildren<CircleCollider2D>();

        //Calculate which pixel to read
        float maxRange = (manager.scale + manager.extraSpaceBetweenObstacles) * (float)Mathf.Floor((float)manager.gridSize / 2f) + manager.randomDistanceRange.y; //(0.5 + 0.1) * 3 + 0.5*0.4
        Vector2 BottomLeft = new Vector2(cameraPosition.x - maxRange, cameraPosition.y - maxRange);
        Vector2 DifferenceFromBottomLeft = new Vector2(springJoint.transform.position.x - BottomLeft.x, springJoint.transform.position.y - BottomLeft.y);
        Vector2Int PixelCoords = new Vector2Int(Mathf.RoundToInt(DifferenceFromBottomLeft.x / (maxRange * 2) * 64f), Mathf.RoundToInt(DifferenceFromBottomLeft.y / (maxRange * 2) * 64f));
        if (PixelCoords.x < 0) { PixelCoords.x += 64; }
        if (PixelCoords.x > 64) { PixelCoords.x -= 64; }
        if (PixelCoords.y < 0) { PixelCoords.y += 64; }
        if (PixelCoords.y > 64) { PixelCoords.y -= 64; }

        isWhite = manager.tex.GetPixel(PixelCoords.x, PixelCoords.y).r == 1;
        isSetUp = true;
    }

    void Update()
    {
        if(!isSetUp) { return; }
        //Update values based on black or white and playerValueProgression
        float progressSelf;
        float progressOther;

		if (isWhite)
		{
            progressSelf = Mathf.Clamp(manager.playerValueProgress - 0.5f, 0, 0.5f) * 2f;
            progressOther = Mathf.Clamp(0.5f - manager.playerValueProgress, 0, 0.5f) * 2f;
        }
		else
		{
            progressOther = Mathf.Clamp(manager.playerValueProgress - 0.5f, 0, 0.5f) * 2f;
            progressSelf = Mathf.Clamp(0.5f - manager.playerValueProgress, 0, 0.5f) * 2f;
        }

        springJoint.frequency = manager.frequencyMiddle + Mathf.Lerp(-(manager.frequencyMiddle - manager.frequencyMin), 0, manager.frequencyCurve.Evaluate(progressSelf)) + Mathf.Lerp(0, manager.frequencyMax - manager.frequencyMiddle, manager.frequencyCurve.Evaluate(1-progressOther));
        col.radius = manager.colliderSizeMiddle + Mathf.Lerp(-(manager.colliderSizeMiddle - manager.colliderSizeMin), 0, manager.colliderSizeCurve.Evaluate(progressSelf)) + Mathf.Lerp(0, manager.colliderSizeMax - manager.colliderSizeMiddle, manager.colliderSizeCurve.Evaluate(1-progressOther));
    }
}
