public abstract class PlayerMode
{
    public virtual PlayerActionRestrictions Restrictions => PlayerActionRestrictions.None;


    public virtual void Enter(){}
    public virtual void Exit(){}
}

[System.Flags]
public enum PlayerActionRestrictions
{
    None = 0,

    RestrictMovement = 1 << 0,
    RestrictJump = 1 << 1,
    RestrictRotation = 1 << 2,
    RestrictAttack = 1 << 3,
    RestrictSkill = 1 << 4,

    All = ~0
}
