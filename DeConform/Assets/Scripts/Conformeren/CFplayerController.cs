using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFplayerController : MonoBehaviour
{
    [SerializeField] private Transform lookAtTarget;

    private Rigidbody2D rb;
    private PlatformManager pM;
    private CFcolorUpdater colorUpdater;

    [Header("Movement Tweaks")]
    [SerializeField] private float forcePerTap;
    [SerializeField] private float maxVelocity;

    [HideInInspector] public bool playerHasTouched = false;

    void Start()
    {
        pM = PlatformManager.instance;
        rb = GetComponent<Rigidbody2D>();
        colorUpdater = GetComponent<CFcolorUpdater>();
        playerHasTouched = false;
    }

	private void Update()
	{
        PlayerInput();
    }

    private void PlayerInput()
	{
        if(Input.touchCount == 0) { return; }

		for (int i = 0; i < Input.touches.Length; i++)
		{
            if(Input.touches[i].phase == TouchPhase.Began)
			{
				if (!playerHasTouched) { colorUpdater.enabled = true; playerHasTouched = true; }
                AddForce(Input.touches[i].position);
            }
		}
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxVelocity);
	}

    private void AddForce(Vector2 touchPosScreen)
	{
        //Get direction
        Vector2 dir = (touchPosScreen - (pM.ScreenResolution / 2f)).normalized;
        //Update direction unit
        lookAtTarget.localEulerAngles = new Vector3(0, 0, (Mathf.Atan2(dir.y, dir.x) * 180f / Mathf.PI) -45f);
        //Apply force
        rb.AddForce(dir * forcePerTap);
	}
}
