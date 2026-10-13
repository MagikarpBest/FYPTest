using System;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(AbilitySystem))]
[RequireComponent(typeof(Sockets))]
public abstract class Character : MonoBehaviour
{
    public Collider Collider { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    public Animator Animator { get; private set; }
    public MovementController Movement { get; private set; }
    public AbilitySystem AbilitySystem { get; private set; }
    public Sockets Sockets { get; private set; }
    
    protected Rigidbody[] ragdollRbs;
    protected Collider[] ragdollCols;

    protected virtual void Awake()
    {
        Collider = GetComponent<Collider>();
        Rigidbody = GetComponent<Rigidbody>();
        Animator = GetComponent<Animator>();
        Movement = GetComponent<MovementController>();
        AbilitySystem = GetComponent<AbilitySystem>();
        Sockets = GetComponent<Sockets>();
        
        ragdollRbs = GetComponentsInChildren<Rigidbody>().Where(rb => rb.gameObject != gameObject).ToArray();
        ragdollCols = GetComponentsInChildren<Collider>().Where(col => col.gameObject != gameObject).ToArray();
    }
}
