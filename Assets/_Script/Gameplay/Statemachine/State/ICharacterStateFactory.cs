public interface ICharacterStateFactory
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
}
