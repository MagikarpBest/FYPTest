using System;
using UnityEngine;

public class PlayerBombSkill : MonoBehaviour
{
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private Transform bombHoldPoint;
    [SerializeField] private PlayerInputManager playerInputManager;

    [Header("Throw Settings")]
    [SerializeField] private float throwForce = 15f;
    [SerializeField] private float upwardForce = 5f;

    private Camera camera;
    private GameObject currentBomb;
    private Rigidbody bombRb;

    private bool isHoldingBomb;

    private void Start()
    {
        camera = Camera.main;
    }

    private void Update()
    {
        if (isHoldingBomb != false && currentBomb != null)
        {
            currentBomb.transform.position = bombHoldPoint.transform.position;
        }
    }

    private void SummonBomb()
    {
        if (isHoldingBomb)
        {
            return;
        }

        // when spawned player is "holding" it
        currentBomb = Instantiate(bombPrefab, bombHoldPoint.position, Quaternion.identity);
        bombRb = currentBomb.GetComponent<Rigidbody>();

        bombRb.isKinematic = true;
        currentBomb.transform.SetParent(bombHoldPoint);
        isHoldingBomb = true;
    }

    private void ThrowBomb()
    {
        if (!isHoldingBomb)
        {
            return;
        }
        currentBomb.transform.SetParent(null);
        bombRb.isKinematic = false;
        Vector3 throwDirection = camera.transform.forward;
        Vector3 force = throwDirection * throwForce + Vector3.up * upwardForce;

        bombRb.AddForce(force, ForceMode.Impulse);

        currentBomb = null;
        bombRb = null;

        isHoldingBomb = false;
    }

    private void OnEnable()
    {
        playerInputManager.OnSkill3Pressed += SummonBomb;
        playerInputManager.OnAttackPressed += ThrowBomb;
    }


}
