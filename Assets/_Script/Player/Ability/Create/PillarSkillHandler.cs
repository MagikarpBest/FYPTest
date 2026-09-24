using System;
using UnityEngine;

namespace Player
{
    [Serializable]
    public class PillarSkillHandler : IPlayerSkillHandler
    {
        private PlayerSkillContext _context;
        private GameObject _pillarPreview;

        // Data defined by ScriptableObject - config only, no scene references allowed here.
        [SerializeField] private float _range = 10f;
        [SerializeField] private float _yOffset = 4.5f; // might consider to delete this.
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

            if (!TryGetTargetPoint(out Vector3 targetPoint))
            {
                _pillarPreview.SetActive(false);
                return;
            }

            _pillarPreview.SetActive(true);
            _pillarPreview.transform.position = targetPoint;
        }

        // TODO: check whether the position available to build.
        public SkillConfirmResult Confirm()
        {
            if (_pillarPreview == null ||
                _pillarPrefab == null)
            {
                return SkillConfirmResult.Fail;
            }

            if (!TryGetTargetPoint(out Vector3 targetPoint))
                return SkillConfirmResult.Fail;

            targetPoint.y -= _yOffset; // TODO: enhance this logic by fetch the prefab's size.
            UnityEngine.Object.Instantiate(_pillarPrefab, targetPoint, Quaternion.identity);

            DestroyPreview();

            return SkillConfirmResult.Finish;
        }

        public void Cancel()
        {
            DestroyPreview();
        }

        private Vector3 GetSpawnPosition()
        {
            if (TryGetTargetPoint(out Vector3 targetPoint))
                return targetPoint;

            return _context.CharacterTransform != null
                ? _context.CharacterTransform.position
                : Vector3.zero;
        }

        private bool TryGetTargetPoint(out Vector3 targetPoint)
        {
            targetPoint = Vector3.zero;

            if (_context == null || _context.Camera == null || _context.CharacterTransform == null)
            {
                return false;
            }

            LineRenderer aimLineRenderer = _context.AimLineRenderer;

            Ray ray = _context.Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            aimLineRenderer.positionCount = 2;

            aimLineRenderer.SetPosition(0, ray.origin);
            aimLineRenderer.SetPosition(1, ray.origin + ray.direction * 100f);

            // Keep the ray long enough to find the target.
            if (!Physics.Raycast(ray, out RaycastHit hit, 100f, _summonLayer))
            {
                return false;
            }

            float distance = Vector3.Distance(_context.CharacterTransform.position, hit.point);

            if (distance > _range)
            {
                return false;
            }

            targetPoint = hit.point;
            Debug.Log("TryGetTargetPoint success: target");
            return true;
        }

        private void DestroyPreview()
        {
            if (_pillarPreview == null)
                return;

            UnityEngine.Object.Destroy(_pillarPreview);
            _pillarPreview = null;
        }

        private Ray DrawCameraRay()
        {
            // Pulled from context instead of a serialized field - this handler is
            // [SerializeReference]'d inside the PlayerSkill ScriptableObject asset, so it
            // must never hold a direct reference to a scene object like a LineRenderer.
            // Different controllers (or none at all) can supply different line renderers,
            // or skip aiming visuals entirely, without touching the asset.
            if (_context?.AimLineRenderer == null || _context.Camera == null)
                return _context.Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            LineRenderer aimLineRenderer = _context.AimLineRenderer;

            Ray ray = _context.Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            aimLineRenderer.positionCount = 2;

            aimLineRenderer.SetPosition(0, ray.origin);
            aimLineRenderer.SetPosition(1, ray.origin + ray.direction * 100f);
            return ray;
        }
    }
}