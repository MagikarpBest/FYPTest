using System;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 DeltaMovement { get; private set; }
    public Collider PlatformCollider;
    private Vector3 lastPosition;
    
    
    // rb
    public Vector3 Velocity { get; private set; }



    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        //DeltaMovement = transform.position - lastPositioncc;
        //lastPosition = transform.position;
        //Debug.Log($"{DeltaMovement}, {lastPosition}");
    }
    
    private void FixedUpdate()
    {
        Velocity = (transform.position - lastPosition) / Time.fixedDeltaTime;
        lastPosition = transform.position;
    }
    
    

}
