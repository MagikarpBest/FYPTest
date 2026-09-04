using DG.Tweening;
using UnityEngine;
using System.Collections;

public class PillarSkill : MonoBehaviour
{
    [SerializeField] private Collider pillarCollider;
    [Header("Settings")]
    [SerializeField] private float riseDuration = 1f;
    private float existDuration = 3f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    

    private void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition + Vector3.up * 4.5f;
        //pillarCollider.enabled = false;

        Rise();
    }

    private void Rise()
    {
        //rise up
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(targetPosition, riseDuration).SetEase(Ease.OutCubic));
        sequence.AppendCallback(() => pillarCollider.enabled = true);
        sequence.AppendInterval(existDuration);
        sequence.Append(transform.DOMove(startPosition, riseDuration).SetEase(Ease.InCubic));
        sequence.OnComplete(() => Destroy(gameObject));
    }
}
