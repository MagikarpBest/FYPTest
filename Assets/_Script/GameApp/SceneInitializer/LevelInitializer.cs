using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class LevelInitializer : SceneInitializer
{
    [SerializeField] private Transform _playerInitialLocation;

    public override void Initialize(System.Action onComplete = null)
    {
        StartCoroutine(InitializePlayerNextFrame(onComplete));
    }

    private IEnumerator InitializePlayerNextFrame(System.Action onComplete)
    {
        yield return null;

        Player player = FindAnyObjectByType<Player>();

        if (player == null || _playerInitialLocation == null)
        {
            Debug.LogWarning("Failed to initialize player position.");
            onComplete?.Invoke();
            yield break;
        }

        CharacterController characterController = player.GetComponent<CharacterController>();
        Rigidbody rigidbody = player.GetComponent<Rigidbody>();
        NavMeshAgent navMeshAgent = player.GetComponent<NavMeshAgent>();

        if (navMeshAgent != null && navMeshAgent.enabled)
        {
            navMeshAgent.Warp(_playerInitialLocation.position);
            navMeshAgent.transform.rotation = _playerInitialLocation.rotation;
        }
        else
        {
            if (characterController != null)
                characterController.enabled = false;

            if (rigidbody != null)
            {
                rigidbody.position = _playerInitialLocation.position;
                rigidbody.rotation = _playerInitialLocation.rotation;
                rigidbody.linearVelocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
            }
            else
            {
                player.transform.SetPositionAndRotation(_playerInitialLocation.position, _playerInitialLocation.rotation);
            }

            if (characterController != null)
                characterController.enabled = true;
        }

        Debug.Log($"Player position set to {player.transform.position}");

        IsTriggered = true;
        onComplete?.Invoke();
    }
}
