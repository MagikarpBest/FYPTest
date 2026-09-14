using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField]private Transform player;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }


    private void Update()
    {
        agent.SetDestination(player.position);
    }
}
