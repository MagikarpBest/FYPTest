using System;
using System.Collections;
using UnityEngine;

public class SlashTest : MonoBehaviour
{
    [SerializeField] private GameObject slash1;
    [SerializeField] private GameObject slash2;

    private void Update()
    {

    }

    private IEnumerator AnimationCallSlash1()
    {
        slash1.SetActive(true);
        yield return new WaitForSeconds(1);
        slash1.SetActive(false);
    }
    private IEnumerator AnimationCallSlash2()
    {
        slash2.SetActive(true);
        yield return new WaitForSeconds(1);
        slash2.SetActive(false);
    }
}
