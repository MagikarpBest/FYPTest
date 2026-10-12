public class PlayerStateFactory : IStateContainer
{
    // Alive State
    public HSMState Alive { get; private set; }

    public HSMState Grounded { get; private set; }
    public HSMState Idle { get; private set; }
    public HSMState Run { get; private set; }

    public HSMState Hit => null;
    public HSMState Airborne { get; private set; }
    public HSMState Jump { get; private set; }
    public HSMState Fall { get; private set; }
    public HSMState Launch { get; private set; }

    public HSMState Attack { get; private set; }
    public HSMState BaseAttack { get; private set; }


    public HSMState Dead { get; private set; }

    public PlayerStateFactory(HierarchicalStateMachine context, Player player)
    {
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
        
    }
}
