using System;
using UnityEngine;
using System.Collections;

public class PillarSkill : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float riseDuration = 1f;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    private void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition + Vector3.up * 4.5f;

        StartCoroutine(Rise());
    }

    private IEnumerator Rise()
    {
        //rise up
        float timer = 0f;

        while (timer < riseDuration)
        {
            timer += Time.deltaTime;
            float time = timer / riseDuration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, time);
            yield return null;

        }
        transform.position = targetPosition;
        
        yield return new WaitForSeconds(4f);
        
        //back down
        timer = 0f;

        while (timer < riseDuration)
        {
            timer += Time.deltaTime;

            float t = timer / riseDuration;
            transform.position = Vector3.Lerp(targetPosition, new Vector3(startPosition.x,startPosition.y-2f,startPosition.z), t);

            yield return null;
        }
        transform.position = startPosition;

        Destroy(gameObject);
    }
}
