public interface IStateContainer
{
    public HSMState Alive { get; }

    public HSMState Grounded { get; }
    public HSMState Idle { get; }
    public HSMState Run { get; }
    public HSMState Hit { get; }

    public HSMState Airborne { get; }
    public HSMState Jump { get; }
    public HSMState Fall { get; }
    public HSMState Launch { get; }

    public HSMState Attack { get; }
    public HSMState BaseAttack { get; }


    public HSMState Dead { get; }
}
