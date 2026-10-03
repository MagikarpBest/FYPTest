public class DeadState : State
{
    private IHasMovement mover;
    public DeadState(HierarchicalStateMachine stateMachine, ICharacter characterContext, State parent,
        ICharacterStateFactory factory) : base(stateMachine, characterContext, parent, factory)
    {
         mover = characterContext as IHasMovement;
    }

    protected override State GetTransition()
    {
        return Factory.Dead;
    }

    protected override void OnEnter()
    {
        //dead
        mover.StopMove();
    }
}
