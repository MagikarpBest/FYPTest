using System;

public interface ISummonedObject
{
    void Despawn();
    event Action<ISummonedObject> OnRemoved; // Call OnDestroy() etc dispose logic.
}