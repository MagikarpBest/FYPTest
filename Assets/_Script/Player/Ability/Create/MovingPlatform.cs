using System;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 DeltaMovement { get; private set; }
    public Collider PlatformCollider;
    private Vector3 lastPosition;

    private void Start()
    {
        lastPosition = transform.position+new Vector3(0,4.5f,0);
    }

    private void Update()
    {
        DeltaMovement = transform.position+new Vector3(0,4.5f,0) - lastPosition;
        lastPosition = transform.position+new Vector3(0,4.5f,0);
        //Debug.Log($"{DeltaMovement}, {lastPosition}");
    }

}
