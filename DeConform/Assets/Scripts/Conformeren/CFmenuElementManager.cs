using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFmenuElementManager : MonoBehaviour
{
    public static CFmenuElementManager instance;

    private void Awake()
	{
        #region Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        #endregion
    }

    private HighlightColorManager hCM;

    public Color ownDiamondOne;
    public Color ownDiamondTwo;
    public Color ownDiamondThree;

    public Vector3 localTransformPositionOne;
    public Vector3 localTransformPositionTwo;
    public Vector3 localTransformPositionThree;

    public Vector3 localTransformCentre;

    // Start is called before the first frame update
    void Start()
    {
        hCM = HighlightColorManager.instance;
        ownDiamondOne = hCM.getHighlightColor();
        ownDiamondTwo = hCM.getHighlightColor();
        ownDiamondThree = hCM.getHighlightColor();
    }
}
