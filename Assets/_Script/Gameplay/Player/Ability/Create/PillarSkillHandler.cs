using System;
using UnityEngine;

[Serializable]
public class PillarSkillHandler : IPlayerSkillHandler
{
    private PlayerSkillContext _context;
    private GameObject _pillarPreview;

    // Data defined by ScriptableObject - config only, no scene references allowed here.
    [SerializeField] private float _range = 10f;
    [SerializeField] private float _buriedDepth = 4.5f;
    [SerializeField] private GameObject _pillarPrefab;
    [SerializeField] private GameObject _pillarPreviewPrefab;
    [SerializeField] private LayerMask _summonLayer;
    // Data defined by ScriptableObject.

    public void Begin(PlayerSkillContext context)
    {
        _context = context;

        if (_pillarPreviewPrefab == null)
            return;

        Vector3 previewPosition = GetSpawnPosition();

        _pillarPreview = UnityEngine.Object.Instantiate(
            _pillarPreviewPrefab,
            previewPosition,
            Quaternion.identity);
    }

    public void Update()
    {
        DrawCameraRay();

        if (_pillarPreview == null)
            return;

        if (!TryGetTarget(out RaycastHit hit))
        {
            _pillarPreview.SetActive(false);
            return;
        }

        _pillarPreview.SetActive(true);

        Quaternion previewRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        _pillarPreview.transform.SetPositionAndRotation(hit.point, previewRotation);
    }

    // TODO: Check whether the position is available to build.
    public SkillConfirmResult Confirm()
    {
        if (_pillarPreview == null ||
            _pillarPrefab == null)
        {
            return SkillConfirmResult.Fail;
        }

        if (!TryGetTarget(out RaycastHit hit))
            return SkillConfirmResult.Fail;

        Vector3 spawnPosition = hit.point - hit.normal * _buriedDepth;

        Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        GameObject pillarObject = UnityEngine.Object.Instantiate(_pillarPrefab, spawnPosition, spawnRotation);

        PillarRiseAnimation pillar = pillarObject.GetComponent<PillarRiseAnimation>();

        if (pillar != null)
        {
            pillar.SetSpawnSurface(hit.point, hit.normal);
        }

        DestroyPreview();

        return SkillConfirmResult.Finish;
    }

    public void Cancel()
    {
        DestroyPreview();
    }

    private Vector3 GetSpawnPosition()
    {
        if (TryGetTarget(out RaycastHit hit))
        {
            return hit.point - hit.normal * _buriedDepth;
        }

        return _context.CharacterTransform != null
            ? _context.CharacterTransform.position
            : Vector3.zero;
    }

    private bool TryGetTarget(out RaycastHit hit)
    {
        hit = default;

        if (_context == null || _context.Camera == null || _context.CharacterTransform == null)
        {
            return false;
        }

        Ray ray = _context.Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (!Physics.Raycast(ray, out hit, 100f, _summonLayer))
        {
            return false;
        }

        float distance = Vector3.Distance(_context.CharacterTransform.position, hit.point);

        if (distance > _range) return false;
        return true;
    }

    private void DestroyPreview()
    {
        if (_pillarPreview == null)
            return;

        UnityEngine.Object.Destroy(_pillarPreview);
        _pillarPreview = null;
    }

    private void DrawCameraRay()
    {
        if (_context?.AimLineRenderer == null || _context.Camera == null)
        {
            return;
        }

        LineRenderer aimLineRenderer = _context.AimLineRenderer;

        Ray ray = _context.Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        aimLineRenderer.positionCount = 2;

        aimLineRenderer.SetPosition(0, ray.origin);
        aimLineRenderer.SetPosition(1, ray.origin + ray.direction * 100f);
    }
    
    // state restriction test, change to manageable by scriptable object

}