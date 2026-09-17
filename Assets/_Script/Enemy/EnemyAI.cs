using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private CharacterController characterController;
    [SerializeField]private Transform player;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;
    }


    private void Update()
    {
        agent.SetDestination(player.position);

        characterController.Move(agent.desiredVelocity*Time.deltaTime);
        Debug.Log($"{agent.desiredVelocity}");
        agent.nextPosition = transform.position;
    }
}
