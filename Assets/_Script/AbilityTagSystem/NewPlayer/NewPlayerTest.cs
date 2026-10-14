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
        Input.OnPrimaryActionPressed += HandlePrimaryAction;
        Input.OnSkill1Pressed += HandleSkill1;
    }

    private void OnDisable()
    {
        Input.OnPrimaryActionPressed -= HandlePrimaryAction;
    }

    private void HandlePrimaryAction()
    {
        AbilitySystem.TryActivateAbilityWithTag("Ability.Attack");
    }

    private void HandleSkill1()
    {
        AbilitySystem.TryActivateAbilityWithTag("Ability.Build");
    }
    
}
