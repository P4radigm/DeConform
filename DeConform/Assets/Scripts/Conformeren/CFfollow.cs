using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFfollow : MonoBehaviour
{
    public Transform targetTransform;
    [SerializeField] private bool followPosition;
    [SerializeField] private bool followRotation;
    public Vector3 accumulatedOffsetToPlayer;

    public bool following;

    private void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (targetTransform == null || !following) { return; }
        if (followPosition)
        {
            transform.position = new Vector3(targetTransform.position.x + accumulatedOffsetToPlayer.x, targetTransform.position.y + accumulatedOffsetToPlayer.y, transform.position.z);
        }

        if (followRotation)
        {
            transform.rotation = targetTransform.rotation;
        }
    }
}
