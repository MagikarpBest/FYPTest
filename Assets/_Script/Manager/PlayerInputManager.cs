using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerInputManager : MonoBehaviour
{
    private InputSystem_Actions input;
    public event Action OnJumpPressed;
    public event Action OnSkillPressed;
    public event Action OnSkill2Pressed;
    public Vector2 MoveInput { get; private set; }


    private void Awake()
    {
        input = new InputSystem_Actions();

        input.Player.Move.performed += Move_Performed;
        input.Player.Move.canceled += Move_Canceled;
        input.Player.Jump.performed += Jump_Performed;
        input.Player.Skill.performed += Skill_Performed;
        input.Player.Skill2.performed += Skill2_Performed;
    }

    private void Skill2_Performed(InputAction.CallbackContext obj)
    {
        OnSkill2Pressed?.Invoke();
    }

    private void Skill_Performed(InputAction.CallbackContext obj)
    {
        OnSkillPressed?.Invoke();
    }

    private void Jump_Performed(InputAction.CallbackContext obj)
    {
        OnJumpPressed?.Invoke();
    }

    private void Move_Performed(InputAction.CallbackContext obj)
    {
        MoveInput = obj.ReadValue<Vector2>();
    }

    private void Move_Canceled(InputAction.CallbackContext obj)
    {
        MoveInput = Vector2.zero;
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
