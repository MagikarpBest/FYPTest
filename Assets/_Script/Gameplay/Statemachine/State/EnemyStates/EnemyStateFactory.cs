public class EnemyStateFactory : ICharacterStateFactory
{
    public State Alive { get; }
    
    public State Grounded { get; }
    public State Idle { get; }
    public State Run { get; }
    public State Hit { get; }
    
    public State Airborne { get; }
    public State Jump => null;
    public State Fall { get; }
    public State Launch { get; }
    
    public State Attack { get; }
    public State BaseAttack { get; }
    
    public State Dead { get; }

    
    public EnemyStateFactory(HierarchicalStateMachine context, Enemy enemy)
    {
        Alive = new EnemyAliveState(context, enemy, null, this);
        Dead = new EnemyDeadState(context, enemy, null, this);

        Grounded = new EnemyGroundedState(context, enemy, Alive, this);
        Idle = new EnemyIdleState(context, enemy, Grounded, this);
        Run = new EnemyRunState(context, enemy, Grounded, this);
        Hit = new EnemyHitState(context, enemy, Alive, this);
        
        Airborne = new EnemyAirborneState(context, enemy, Alive, this);
        Fall = new EnemyFallState(context, enemy, Airborne, this);
        Launch = new EnemyLaunchState(context, enemy, Airborne, this);

        Attack = new EnemyAttackState(context, enemy, Alive, this);
        BaseAttack = new EnemyBaseAttackState(context, enemy, Attack, this);
    }
}
