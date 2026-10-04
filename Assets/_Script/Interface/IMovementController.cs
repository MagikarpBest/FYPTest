using UnityEngine;

public interface IMovementController
{
    public bool IsGrounded { get; }
    public bool IsMoving { get; }
    public bool IsFalling { get; }
    public void StopMove();
}
