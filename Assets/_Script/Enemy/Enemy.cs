using System;
using UnityEngine;
using UnityEngine.AI;

// dont touch yet
public class Enemy : MonoBehaviour, ICharacter, IHasMovement, IHasAttack
{
    private Rigidbody rigidBody;
    private NavMeshAgent navMeshAgent;

    public EnemyAI enemyAI;
    public bool IsGrounded { get; }
    public bool IsMoving { get; }

    public void Move()
    {
        throw new NotImplementedException();
    }

    public void StopMove()
    {
        throw new System.NotImplementedException();
    }

    public bool isAttacking { get; }
    public bool isComboQueued { get; }

    public void HandleAtack()
    {
        throw new System.NotImplementedException();
    }

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        enemyAI = GetComponent<EnemyAI>();

        enemyAI.Init(navMeshAgent, rigidBody);

    }
}
