using UnityEngine;

public class PlayerModeController : MonoBehaviour
{
    public PlayerMode CurrentMode { get; private set; }

    public NormalMode Normal { get; private set; }
    public BuildMode Build { get; private set; }
    
    // no need attack?
    public AttackMode Attack { get; private set; }

    private void Awake()
    {
        Normal = new NormalMode();
        Build = new BuildMode();
        Attack = new AttackMode();
        
        ChangeMode(Normal);
    }
    
    public void ChangeMode(PlayerMode newMode)
    {
        CurrentMode?.Exit();
        Debug.Log($"From {CurrentMode} -> {newMode}");
        CurrentMode = newMode;
        
        CurrentMode?.Enter();
    }
}
