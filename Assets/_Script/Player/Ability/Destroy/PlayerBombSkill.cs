using DG.Tweening;
using UnityEngine;

public class PlayerBombSkill : MonoBehaviour
{
    [SerializeField] private Bomb bombPrefab;
    [SerializeField] private Transform bombHoldPoint;
    [SerializeField] private PlayerInputManager playerInputManager;

    [Header("Throw Settings")]
    [SerializeField] private float throwForce = 15f;
    [SerializeField] private float upwardForce = 5f;

    private Camera camera;
    private Bomb currentBomb;
    private bool hasThrown = false;


    private void Start()
    {
        camera = Camera.main;
    }
    

    private void SummonOrExplode()
    {
        if (currentBomb == null)
        {
            
            // when spawned player is "holding" it
            currentBomb = Instantiate(bombPrefab, bombHoldPoint.position, Quaternion.identity);
            currentBomb.Hold(bombHoldPoint);

            hasThrown = false;
            Debug.Log("Summoned bomb");
            return;
        }

        // bomb exists then explode
        currentBomb.Explode();
        Debug.Log("Bomb exploded");
        currentBomb = null;
    }

    private void ThrowBomb()
    {
        if (currentBomb == null || hasThrown == true)
        {
            return;
        }
        Vector3 direction = camera.transform.forward;

        currentBomb.Throw(direction, throwForce, upwardForce);
        
        RotatePlayer(direction);

        hasThrown = true;
        Debug.Log("Thrown exploded");
    }

    private void RotatePlayer(Vector3 direction)
    {
        direction.y = 0;

        if (direction.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform
            .DORotateQuaternion(targetRotation, 0.2f)
            .SetEase(Ease.OutSine);
    }

    private void OnEnable()
    {
        playerInputManager.OnSkill3Pressed += SummonOrExplode;
        playerInputManager.OnAttackPressed += ThrowBomb;

    }

    private void OnDisable()
    {
        playerInputManager.OnSkill3Pressed -= SummonOrExplode;
        playerInputManager.OnAttackPressed -= ThrowBomb;

    }


}
