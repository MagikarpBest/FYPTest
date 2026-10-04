using System;
using UnityEngine;

// State machine test stuff
public interface ICharacter 
{
    PlayerActionRestrictions Restrictions { get; }
}
