using UnityEngine;

public interface IMovementController
{
    //these bools are useless because it just makes the statemachine pointless use your states as condition checks its the whole point of a state machine!
    public bool IsGrounded { get; }
    // public bool IsMoving { get; }
    // public bool IsFalling { get; }
    public void StopMove();
}
