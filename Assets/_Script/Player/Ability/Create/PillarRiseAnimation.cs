using DG.Tweening;
using System;
using UnityEngine;

public class PillarRiseAnimation : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidBody;
    [Header("Settings")]
    [SerializeField] private float riseDuration = 1f;
    private float existDuration = 3f;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    public event Action OnPillarRising;
    public event Action OnPillarFalling;

    
    private void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition + Vector3.up * 6.7f;

        Rise();
    }

    private void Rise()
    {
        //rise up
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() => OnPillarRising?.Invoke());
        sequence.Append(rigidBody.DOMove(targetPosition, riseDuration).SetEase(Ease.OutCubic).SetUpdate(UpdateType.Fixed));
        sequence.AppendInterval(existDuration);
        sequence.AppendCallback(() => OnPillarFalling?.Invoke());
        sequence.Append(rigidBody.DOMove(startPosition, riseDuration).SetEase(Ease.InCubic).SetUpdate(UpdateType.Fixed));
        sequence.OnComplete(() => Destroy(gameObject));
    }
    
    
}
