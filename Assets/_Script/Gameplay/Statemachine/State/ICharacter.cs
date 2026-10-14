using System;
using UnityEngine;

// State machine test stuff
public interface ICharacter 
{
    Transform Transform { get; }

    PlayerActionRestrictions Restrictions { get; }

    public void DisableRagdoll();
    public void EnableRagdoll();
}
