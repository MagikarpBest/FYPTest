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

        _pillarPreview = UnityEngine.Object.Instantiate(
            _pillarPreviewPrefab,
            GetSpawnPosition(),
            Quaternion.identity);
    }

    public void Update()
    {
        PlayerSkillHandlerHelper.DrawCameraRay(_context);

        if (_pillarPreview == null)
            return;

        if (!TryGetTarget(out RaycastHit hit))
        {
            _pillarPreview.SetActive(false);
            return;
        }

        _pillarPreview.SetActive(true);
        _pillarPreview.transform.SetPositionAndRotation(
            hit.point,
            Quaternion.FromToRotation(Vector3.up, hit.normal));
    }

    // TODO: Check whether the position is available to build.
    public SkillConfirmResult Confirm(out ISummonedObject summonedObject)
    {
        summonedObject = null;
        if (_pillarPreview == null || _pillarPrefab == null)
            return SkillConfirmResult.Fail;

        if (!TryGetTarget(out RaycastHit hit))
            return SkillConfirmResult.Fail;

        Vector3 spawnPosition = hit.point - hit.normal * _buriedDepth;
        Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        GameObject pillarObject = UnityEngine.Object.Instantiate(_pillarPrefab, spawnPosition, spawnRotation);

        summonedObject = pillarObject.GetComponent<ISummonedObject>();
        PillarRiseAnimation pillar = pillarObject.GetComponent<PillarRiseAnimation>();
        if (pillar != null)
            pillar.SetSpawnSurface(hit.point, hit.normal);

        PlayerSkillHandlerHelper.DestroyAndClear(ref _pillarPreview);

        return SkillConfirmResult.Finish;
    }

    public void Cancel()
    {
        PlayerSkillHandlerHelper.DestroyAndClear(ref _pillarPreview);
    }

    private Vector3 GetSpawnPosition()
    {
        if (TryGetTarget(out RaycastHit hit))
            return hit.point - hit.normal * _buriedDepth;

        return _context.CharacterTransform != null
            ? _context.CharacterTransform.position
            : Vector3.zero;
    }

    private bool TryGetTarget(out RaycastHit hit)
    {
        if (!PlayerSkillHandlerHelper.TryRaycastFromCamera(_context, PlayerSkillHandlerHelper.DefaultAimDistance, _summonLayer, out hit))
            return false;

        return PlayerSkillHandlerHelper.IsWithinRange(_context, hit.point, _range);
    }

    // state restriction test, change to manageable by scriptable object
}