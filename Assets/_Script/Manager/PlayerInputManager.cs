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
    public event Action OnEscapePressed;
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

        input.Global.Escape.performed += Escape_Performed; // Handle Escape key for interrupting game interaction or opening the pause menu.

        // Assume testing gameplay most of the time. Might remove later.
        ToggleCursor(false);
    }

    /// <summary>
    /// Track when enter UI input mode.
    /// </summary>
    private void ToggleCursor(bool active)
    {
        Cursor.visible = active;
        Cursor.lockState = active ? CursorLockMode.Confined : CursorLockMode.Locked;
    }

    private void HandleUIActiveChanged(bool isUIActive)
    {
        ToggleCursor(isUIActive);

        if (isUIActive)
        {
            input.Player.Disable();
            input.UI.Enable();
        }
        else
        {
            input.UI.Disable();
            input.Player.Enable();
        }
    }

    private void Escape_Performed(InputAction.CallbackContext obj)
    {
        OnEscapePressed?.Invoke();
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
        input.Global.Enable();
        input.Player.Enable();
        GameScreenManager.OnUIActiveChanged += HandleUIActiveChanged;
    }

    private void OnDisable()
    {
        input.Global.Disable();
        input.Player.Disable();
        input.UI.Disable();
        GameScreenManager.OnUIActiveChanged -= HandleUIActiveChanged;
    }
}
