using UnityEngine;

public interface IMovementController
{
    public void GroundCheck();
    public void HandleGravity();
    public void AddForce(Vector3 force);
    public void StopMovement();
    public void HandleRotation();
    public void SlopeCorrection();
    

}
