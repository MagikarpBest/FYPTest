public class EnemyStateFactory : IStateContainer
{
    public HSMState Alive { get; }
    
    public HSMState Grounded { get; }
    public HSMState Idle { get; }
    public HSMState Run { get; }
    public HSMState Hit { get; }
    
    public HSMState Airborne { get; }
    public HSMState Jump => null;
    public HSMState Fall { get; }
    public HSMState Launch { get; }
    
    public HSMState Attack { get; }
    public HSMState BaseAttack { get; }
    
    public HSMState Dead { get; }

    
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
