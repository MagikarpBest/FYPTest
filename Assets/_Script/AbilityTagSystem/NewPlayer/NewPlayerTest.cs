using UnityEngine;

public class NewPlayerTest : Character, ILaunchable
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
        Input.OnSkill2Pressed += HandleSkill2;
    }

    private void OnDisable()
    {
        Input.OnPrimaryActionPressed -= HandlePrimaryAction;
        Input.OnSkill1Pressed -= HandleSkill1;
        Input.OnSkill2Pressed -= HandleSkill2;
    }

    private void HandlePrimaryAction()
    {
        AbilitySystem.TryActivateAbilityWithTag("Ability.Attack");
    }

    private void HandleSkill1()
    {
        AbilitySystem.TryActivateAbilityWithTag("Ability.Build");
    }
    
    private void HandleSkill2()
    {
        AbilitySystem.TryActivateAbilityWithTag("Ability.Bomb");
    }

    public void Launch(Vector3 force)
    {
        Movement.ApplyForce(force);
        Debug.Log("Launch");
    }
    
}
