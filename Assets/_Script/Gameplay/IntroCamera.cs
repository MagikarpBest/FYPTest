using System;
using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class IntroCamera : MonoBehaviour
{
    [SerializeField] CinemachineCamera introStartCamera;
    [SerializeField] private GameObject timeline;


    private Coroutine Intro;

    private void Update()
    {
        if(Keyboard.current.zKey.wasPressedThisFrame)
        {
            timeline.SetActive(true);
            introStartCamera.gameObject.SetActive(true);
        }
        if(Keyboard.current.vKey.wasPressedThisFrame)
        {
            introStartCamera.Priority = 0;
        }
    }





}
