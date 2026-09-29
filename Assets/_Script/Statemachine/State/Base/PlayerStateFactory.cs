public class PlayerStateFactory
{
    private HierarchicalStateMachine context;
    private ICharacter player;

    // Alive State
    public State Alive { get; private set; }

    public State Grounded { get; private set; }
    public State Idle { get; private set; }
    public State Run { get; private set; }

    public State Airborne { get; private set; }
    public State Jump { get; private set; }
    public State Fall { get; private set; }
    public State Launch { get; private set; }

    public State Attack { get; private set; }
    public State BaseAttack { get; private set; }


    public State Dead { get; private set; }

    public PlayerStateFactory(HierarchicalStateMachine context, Player player)
    {
        this.context = context;
        this.player = player;

        // Initialize the tree structure here
        Alive = new AliveState(context, player, null, this);
        Dead = new DeadState(context, player, null, this);

        Grounded = new GroundedState(context, player, Alive, this);
        Airborne = new AirborneState(context, player, Alive, this);

        Idle = new IdleState(context, player, Grounded, this);
        Run = new RunState(context, player, Grounded, this);

        Jump = new JumpState(context, player, Airborne, this);
        Fall = new FallState(context, player, Airborne, this);
        Launch = new LaunchState(context, player, Airborne, this);

        Attack = new AttackState(context, player, Alive, this);
        BaseAttack = new BaseAttackState(context, player, Attack, this);
    }
}
