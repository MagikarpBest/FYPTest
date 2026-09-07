using System;
using UnityEngine;

public class LaunchingPillarSkill : MonoBehaviour
{
    [SerializeField] private PillarRiseAnimation pillarRiseAnimation;
    [SerializeField] float launchPower = 30f;

    private bool isRising;

    // bruh ts just created alot filler code just because iw anted to use event
    private void OnEnable()
    {
        pillarRiseAnimation.OnPillarRising += HandlePillarRising;
        pillarRiseAnimation.OnPillarFalling += HandlePillarFalling;
    }

    private void OnDisable()
    {
        pillarRiseAnimation.OnPillarRising -= HandlePillarRising;
        pillarRiseAnimation.OnPillarFalling -= HandlePillarFalling;
    }
    
    private void HandlePillarRising()
    {
        isRising = true;
    }
    
    private void HandlePillarFalling()
    {
        isRising = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // only launch player if its rising, dont want stasis pillar to launch player
        if(!isRising)
        {
            return;
        }
        ILaunchable launchable = other.GetComponent<ILaunchable>();

        if (launchable != null)
        {
            launchable.Launch(Vector3.up * launchPower);
        }
    }
}
