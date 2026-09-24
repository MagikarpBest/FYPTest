using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerInputManager : MonoBehaviour
{
    private InputSystem_Actions input;
    public event Action OnJumpPressed;
    public event Action OnSkillPressed;
    public event Action OnSkill2Pressed;
    public event Action OnSkill3Pressed;
    public event Action OnSkillCancelPressed;
    public event Action OnAttackPressed;
    public Vector2 MoveInput { get; private set; }


    private void Awake()
    {
        input = new InputSystem_Actions();

        input.Player.Move.performed += Move_Performed;
        input.Player.Move.canceled += Move_Canceled;
        input.Player.Jump.performed += Jump_Performed;
        input.Player.Skill.performed += Skill_Performed;
        input.Player.Skill2.performed += Skill2_Performed;
        input.Player.Skill3.performed += Skill3_Performed;
        input.Player.Attack.performed += Attack_Performed;
        input.Player.SkillCancel.performed += SkillCancel_Performed;
    }




    private void Move_Performed(InputAction.CallbackContext obj)
    {
        MoveInput = obj.ReadValue<Vector2>();
    }
    
    private void Move_Canceled(InputAction.CallbackContext obj)
    {
        MoveInput = Vector2.zero;
    }
    
    private void Jump_Performed(InputAction.CallbackContext obj)
    {
        OnJumpPressed?.Invoke();
    }
    
    private void Skill_Performed(InputAction.CallbackContext obj)
    {
        OnSkillPressed?.Invoke();
    }
    
    private void Skill2_Performed(InputAction.CallbackContext obj)
    {
        OnSkill2Pressed?.Invoke();
    }

    private void Skill3_Performed(InputAction.CallbackContext obj)
    {
        OnSkill3Pressed?.Invoke();
    }

    private void SkillCancel_Performed(InputAction.CallbackContext obj)
    {
        OnSkillCancelPressed?.Invoke();
    }

    private void Attack_Performed(InputAction.CallbackContext obj)
    {
        OnAttackPressed?.Invoke();
    }

    private void Update()
    {
        MoveInput = input.Player.Move.ReadValue<Vector2>();
    }
    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }
}
