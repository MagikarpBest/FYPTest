using UnityEngine;

public class NewPlayerTest : Character
{
    public PlayerInputManager Input { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Input = GetComponent<PlayerInputManager>();

    }

    private void OnEnable()
    {
        Input.OnAttackPressed += HandleAttack;
    }

    private void OnDisable()
    {
        Input.OnAttackPressed -= HandleAttack;
    }

    void HandleAttack()
    {
        AbilitySystem.TryActivateAbilityWithTag("Ability.Attack");
    }
    
}
