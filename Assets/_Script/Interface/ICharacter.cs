using System;

// State machine test stuff
public interface ICharacter 
{

}

public interface IHasMovement
{
    bool IsGrounded { get; }
    bool IsMoving { get; }
    void Move();
    void StopMove();
}

public interface IHasAttack
{
    bool isAttacking { get; }
    bool isComboQueued { get; }
    void HandleAtack(); // testing combo time frame, idk enemy need or not
}

public interface ICanUseSkills
{
    void SetSkillUsable(bool usable);
}

public interface IHasInput
{
    event Action OnJumpPressed;
    event Action OnAttackPressed;
}
