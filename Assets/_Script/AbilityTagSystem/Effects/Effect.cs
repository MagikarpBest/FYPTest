using UnityEngine;

public class Effect : MonoBehaviour
{
    public readonly EffectDefinitionSO Definition;

    public bool IsActive;

    public float durationTimer;
    public float periodTimer;


    public void OnApply()
    {
        IsActive = true;
        durationTimer = Definition.Duration;
        periodTimer = Definition.ApplyOnFirstTickOfPeriod ? Definition.Period : 0f;
    }
    
    public void Tick(float deltaTime)
    {
        //tick timers
        periodTimer += deltaTime;
        durationTimer -= deltaTime;
        
        //period
        if (periodTimer >= Definition.Period)
        {
            periodTimer -= Definition.Period; //reset timer 
            //apply modifiers
        }
    
        //duration
        if (durationTimer <= 0)
        {
            //mark for removal
            IsActive = false;
        }
    }
}
