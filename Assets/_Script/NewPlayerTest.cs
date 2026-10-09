using UnityEngine;
using System.Linq;

public class NewPlayerTest : MonoBehaviour
{
    public Collider Collider { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    public Animator Animator { get; private set; }
    public PlayerInputManager Input { get; private set; }
    public PlayerMovement Movement { get; private set; }
    
    private SkinnedMeshRenderer renderer;
    
    private Rigidbody[] ragdollRbs;
    private Collider[] ragdollCols;

    private void Awake()
    {
        Collider = GetComponent<Collider>();
        Rigidbody = GetComponent<Rigidbody>();
        Animator = GetComponent<Animator>();

        Input = FindFirstObjectByType<PlayerInputManager>();
        Movement = GetComponent<PlayerMovement>();
        
        renderer = GetComponent<SkinnedMeshRenderer>();
        
        ragdollRbs = GetComponentsInChildren<Rigidbody>().Where(rb => rb.gameObject != gameObject).ToArray();
        ragdollCols = GetComponentsInChildren<Collider>() .Where(col => col.gameObject != gameObject) .ToArray();
    }
    
}
