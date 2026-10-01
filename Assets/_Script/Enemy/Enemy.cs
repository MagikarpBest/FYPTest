using System;
using UnityEngine;

// dont touch yet
public class Enemy : MonoBehaviour //ICharacter,IHasMovement,IHasAttack
{
    public bool IsGrounded { get; }
    public void StopMove()
    {
        throw new System.NotImplementedException();
    }

    public bool isAttacking { get; }
    public void HandleAtack()
    {
        throw new System.NotImplementedException();
    }

    private void Awake()
    {
        
    }
}
