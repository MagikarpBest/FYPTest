using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Build", menuName = "AbilitySystem/Abilities/Build")]
public class BuildAbilitySO : AbilityDefinitionSO
{
    public async override Awaitable OnActivate(CancellationToken token)
    {
        await Awaitable.NextFrameAsync(token);
    }

    public override void OnEnd()
    {
        
    }

    public override void OnCancel()
    {
        
    }

    public override void Cleanup()
    {
        
    }
}
