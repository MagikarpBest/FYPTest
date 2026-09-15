using System;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 DeltaMovement { get; private set; }
    public Collider PlatformCollider;
    private Vector3 lastPosition;

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        DeltaMovement = transform.position - lastPosition;
        lastPosition = transform.position;
        //Debug.Log($"{DeltaMovement}, {lastPosition}");
    }

}
