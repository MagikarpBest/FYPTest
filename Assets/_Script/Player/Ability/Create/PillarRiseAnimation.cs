using DG.Tweening;
using System;
using UnityEngine;

public class PillarRiseAnimation : MonoBehaviour
{
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
        targetPosition = startPosition + Vector3.up * 4.5f;

        Rise();
    }

    private void Rise()
    {
        //rise up
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() => OnPillarRising?.Invoke());
        sequence.Append(transform.DOMove(targetPosition, riseDuration).SetEase(Ease.OutCubic));
        sequence.AppendInterval(existDuration);
        sequence.AppendCallback(() => OnPillarFalling?.Invoke());
        sequence.Append(transform.DOMove(startPosition, riseDuration).SetEase(Ease.InCubic));
        sequence.OnComplete(() => Destroy(gameObject));
    }
    
    
}
