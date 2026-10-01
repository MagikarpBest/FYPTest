public class EnemyStateFactory : ICharacterStateFactory
{
    public State Alive { get; }
    public State Grounded { get; }
    public State Idle { get; }
    public State Run { get; }
    public State Airborne { get; }
    public State Jump { get; }
    public State Fall { get; }
    public State Launch { get; }
    public State Attack { get; }
    public State BaseAttack { get; }
    public State Dead { get; }

    public EnemyStateFactory(HierarchicalStateMachine context, ICharacter enemy)
    {
        Alive = new AliveState(context, enemy, null, this);
        Dead = new DeadState(context, enemy, null, this);

        Grounded = new GroundedState(context, enemy, Alive, this);
        Airborne = new AirborneState(context, enemy, Alive, this);

        Idle = new IdleState(context, enemy, Grounded, this);
        Run = new RunState(context, enemy, Grounded, this);

        Jump = new JumpState(context, enemy, Airborne, this);
        Fall = new FallState(context, enemy, Airborne, this);
        Launch = new LaunchState(context, enemy, Airborne, this);

        Attack = new AttackState(context, enemy, Alive, this);
        BaseAttack = new BaseAttackState(context, enemy, Attack, this);
    }
}
