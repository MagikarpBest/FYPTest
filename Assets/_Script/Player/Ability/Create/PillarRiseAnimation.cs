using DG.Tweening;
using System;
using UnityEngine;

public class PillarRiseAnimation : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidBody;
    [Header("Settings")]
    [SerializeField] private float riseDuration = 1f;
    [SerializeField] private float pillarHeight = 4.5f;
    [SerializeField] private LayerMask groundLayer;
    private float existDuration = 3f;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    private Vector3 surfaceNormal = Vector3.up;
    private bool initialized;

    public event Action OnPillarRising;
    public event Action OnPillarFalling;




    public void SetSpawnSurface(Vector3 surfacePoint, Vector3 normal)
    {
        surfaceNormal = normal;

        startPosition = rigidBody.position;

        rigidBody.rotation =
            Quaternion.FromToRotation(
                Vector3.up,
                normal
            );

        targetPosition =
            startPosition + normal * pillarHeight;

        initialized = true;

        Rise();
    }

    private void Rise()
    {
        //rise up
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() => OnPillarRising?.Invoke());

        sequence.Append(
            rigidBody.DOMove(targetPosition, riseDuration)
                .SetEase(Ease.OutCubic).SetUpdate(UpdateType.Fixed)
        );
        sequence.AppendInterval(existDuration);

        sequence.AppendCallback(() => OnPillarFalling?.Invoke());
        sequence.Append(
            rigidBody.DOMove(startPosition, riseDuration)
                .SetEase(Ease.InCubic)
                .SetUpdate(UpdateType.Fixed)
        );
        sequence.OnComplete(() => Destroy(gameObject));
    }


}
