using System;
using UnityEngine;

public class Debris : MonoBehaviour
{
    private Rigidbody[] rbs;
    private MeshRenderer[] renderers;
    
    private void Awake()
    {
        rbs = GetComponentsInChildren<Rigidbody>();
        renderers = GetComponentsInChildren<MeshRenderer>();
    }
}
