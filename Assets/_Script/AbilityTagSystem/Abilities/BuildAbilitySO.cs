using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

[CreateAssetMenu(fileName = "Build", menuName = "AbilitySystem/Abilities/Build")]
public class BuildAbilitySO : AbilityDefinitionSO
{
    [SerializeField] private float range = 10f;
    [SerializeField] private float maxBuildAngle = 100f;
    [SerializeField] private GameObject pillarPrefab;
    [SerializeField] private GameObject pillarPreviewPrefab;
    [SerializeField] private LayerMask buildLayer;
    [SerializeField] private LayerMask excludeLayer; 
    
    private PlayerInputManager input;
    private TaskCompletionSource<bool> buildFinishedTcs;
    
    private GameObject pillarPreview;

    private RaycastHit hit = default;
    private bool validPos;

    public async override Awaitable OnActivate(CancellationToken token)
    {
        if (!ctx.Character.TryGetComponent<PlayerInputManager>(out input)) return;
        if (pillarPrefab == null || pillarPreviewPrefab == null) return;
        
        buildFinishedTcs = new TaskCompletionSource<bool>();
        token.Register(() => buildFinishedTcs.TrySetCanceled()); //when token canceled set buildFinishedTcs to cancel also

        input.OnPrimaryActionPressed += TrySpawnPillar;
        input.OnSecondaryActionPressed += EndBuild;
        
        //spawn preview
        float cosMaxBuildAngle = Mathf.Cos(maxBuildAngle * Mathf.Deg2Rad);
        pillarPreview = Instantiate(pillarPreviewPrefab, ctx.Character.transform);
        pillarPreview.gameObject.SetActive(false);
        Renderer pillarPreviewRenderer = pillarPreview.GetComponent<Renderer>();
        Material pillarPreviewMaterial = pillarPreviewRenderer.material;

        while (!buildFinishedTcs.Task.IsCompleted)
        {
            //if aiming at something and within range
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out hit, range + 1f, buildLayer, QueryTriggerInteraction.Ignore))
            {
                validPos =
                    (excludeLayer.value & (1 << hit.collider.gameObject.layer)) == 0
                    && Vector3.Distance(ctx.Character.transform.position, hit.point) <= range
                    && Vector3.Dot(Vector3.up, hit.normal) >= cosMaxBuildAngle;
                
                Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
                pillarPreview.transform.SetPositionAndRotation(hit.point, rot);
                pillarPreviewMaterial.SetColor("_BaseColor", validPos ? Color.cyan : Color.red);
                pillarPreview.SetActive(true);
            }
            else
            {
                pillarPreview.SetActive(false);
            }
            
            await Awaitable.NextFrameAsync(token);
        }
    }

    private void EndBuild() => buildFinishedTcs.TrySetResult(true);
    
    private void TrySpawnPillar()
    {
        if (hit.collider != null && validPos)
        {
            Vector3 pos = hit.point - hit.normal * 4.9647f;
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            Instantiate(pillarPrefab, pos , rot);
            buildFinishedTcs.TrySetResult(true);
        }
    }
    
    public override void Cleanup()
    {
        input.OnPrimaryActionPressed -= TrySpawnPillar;
        input.OnSecondaryActionPressed -= EndBuild;
        Destroy(pillarPreview);
    }
}
