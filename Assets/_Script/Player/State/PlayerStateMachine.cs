using UnityEngine;

public class PlayerStateMachine :MonoBehaviour
{
    public PlayerState CurrentState { get; private set; }
    public PlayerInputManager Input { get; private set; }
    public PlayerMovementRB Movement { get; private set; }

    
    private void Awake()
    {
        Input = FindFirstObjectByType<PlayerInputManager>();
        Movement = FindFirstObjectByType<PlayerMovementRB>();

    }
    public void ChangeState(PlayerState newState)
    {
        CurrentState?.Exit();

        CurrentState = newState;
        
        CurrentState?.Enter();
    }
    
    public void Update()
    {
        CurrentState?.Update();
    }


    public void FixedUpdate()
    {
        CurrentState?.FixedUpdate();
    }
}
