using System;
using UnityEngine;

public class TagTest : MonoBehaviour
{
    [SerializeField] GameplayTagContainer tags;

    private void Start()
    {
        Debug.Log($"Movement.Grounded.Run: {tags.HasTag("Movement.Grounded.Run")}");
        Debug.Log($"Movement: {tags.HasTag("Movement")}");
        Debug.Log($"Movement.Airborne: {tags.HasTag("Movement.Airborne")}");
        Debug.Log($"Movement.Airborne.Jump: {tags.HasTag("Movement.Airborne.Jump")}");
        Debug.Log($"Jump: {tags.HasTag("Jump")}");
    }
}
