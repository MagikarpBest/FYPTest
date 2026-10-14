// using System;
// using UnityEngine;
//
// /// <summary>
// /// Aim at a point, confirm, and a rock materializes above that point and falls down onto it.
// /// </summary>
// [Serializable]
// public class RockSkillHandler : 
// {
//     private struct TargetInfo
//     {
//         public Vector3 Point;
//         public Vector3 Normal;
//         public float SpawnHeight;
//     }
//
//     private GameObject _impactMarker;
//
//     // Data defined by ScriptableObject - config only, no scene references allowed here.
//     [Header("Targeting")]
//     [SerializeField] private float _range = 15f;
//     [SerializeField] private LayerMask _summonLayer;
//     [Tooltip("Surfaces steeper than this (walls) or facing down (roofs) can't be aimed at.")]
//     [SerializeField, Range(0f, 89f)] private float _maxGroundAngle = 45f;
//     [Tooltip("When aiming at the sky or past max range, how far above the character the ground search starts.")]
//     [SerializeField] private float _groundSearchUp = 10f;
//     [Tooltip("How far down the ground search goes.")]
//     [SerializeField] private float _groundSearchDown = 40f;
//
//     [Header("Wall / Ceiling Clearance")]
//     [Tooltip("Footprint radius kept free of walls. Usually the rock's radius or the impact radius.")]
//     [SerializeField] private float _clearanceRadius = 2f;
//     [Tooltip("Height above the floor where wall probes are fired from (should be above small steps).")]
//     [SerializeField] private float _probeHeight = 0.5f;
//     [SerializeField, Range(4, 32)] private int _probeCount = 12;
//     [Tooltip("How many push-out passes are done (more = better corner handling).")]
//     [SerializeField, Range(1, 8)] private int _resolveIterations = 4;
//
//     [Header("Rock")]
//     [SerializeField] private GameObject _rockPrefab;
//     [SerializeField] private GameObject _impactMarkerPrefab;
//     [Tooltip("How high above the aimed point the rock appears.")]
//     [SerializeField] private float _spawnHeight = 10f;
//     [Tooltip("If the ceiling is lower than this, the rock can't be summoned.")]
//     [SerializeField] private float _minSpawnHeight = 3f;
//     [Tooltip("Gap kept between the rock and a ceiling when the height is clamped.")]
//     [SerializeField] private float _ceilingMargin = 1f;
//
//     [Header("Rock Behaviour")]
//     [SerializeField] private float _materializeDuration = 0.4f;
//     [SerializeField] private float _hoverDuration = 0.4f;
//     [SerializeField] private float _initialFallSpeed = 15f;
//     [SerializeField] private float _damage = 5f;
//     [SerializeField] private float _impactRadius = 3f;
//     [SerializeField] private float _impactForce = 8f;
//     [SerializeField] private DamageType _damageType = DamageType.Physical;
//     [SerializeField] private AttackPowerLevel _attackPowerLevel = AttackPowerLevel.Heavy;
//     // Data defined by ScriptableObject.
//
//     private const float AimDistance = PlayerSkillHandlerHelper.DefaultAimDistance;
//
//     #region IPlayerSkillHandler
//
//     public void Begin(PlayerSkillContext context)
//     {
//         _context = context;
//
//         if (_impactMarkerPrefab == null)
//             return;
//
//         _impactMarker = UnityEngine.Object.Instantiate(_impactMarkerPrefab);
//         _impactMarker.SetActive(false);
//     }
//
//     public void Update()
//     {
//         PlayerSkillHandlerHelper.DrawCameraRay(_context, AimDistance);
//
//         if (_impactMarker == null)
//             return;
//
//         if (!TryGetTarget(out TargetInfo target))
//         {
//             _impactMarker.SetActive(false);
//             return;
//         }
//
//         _impactMarker.SetActive(true);
//         _impactMarker.transform.SetPositionAndRotation(
//             target.Point + target.Normal * 0.02f,
//             Quaternion.FromToRotation(Vector3.up, target.Normal));
//         _impactMarker.transform.localScale = new Vector3(1.0f * 2f, 1f, 1.0f * 2f);
//     }
//
//     public SkillConfirmResult Confirm(out ISummonedObject summonedObject)
//     {
//         summonedObject = null;
//
//         if (_rockPrefab == null)
//             return SkillConfirmResult.Fail;
//
//         if (!TryGetTarget(out TargetInfo target))
//             return SkillConfirmResult.Fail;
//
//         Vector3 spawnPosition = target.Point + Vector3.up * target.SpawnHeight;
//         GameObject rockObject = UnityEngine.Object.Instantiate(_rockPrefab, spawnPosition, Quaternion.identity);
//
//         summonedObject = rockObject.GetComponent<ISummonedObject>();
//         RockBehaviour rock = rockObject.GetComponent<RockBehaviour>();
//         if (rock == null)
//         {
//             Debug.LogWarning($"{nameof(RockSkillHandler)}: rock prefab has no {nameof(RockBehaviour)}.");
//             UnityEngine.Object.Destroy(rockObject);
//             return SkillConfirmResult.Fail;
//         }
//
//         rock.Init(
//             new DamageData(_damage, _damageType, _attackPowerLevel),
//             _impactRadius,
//             _impactForce,
//             _initialFallSpeed,
//             _materializeDuration,
//             _hoverDuration
//             );
//
//         PlayerSkillHandlerHelper.DestroyAndClear(ref _impactMarker);
//
//         return SkillConfirmResult.Finish;
//     }
//
//     public void Cancel()
//     {
//         PlayerSkillHandlerHelper.DestroyAndClear(ref _impactMarker);
//     }
//
//     #endregion
//
//     // TODO: consider to move this pieces of targeting region logic into playerskillhandlerhelper.cs...
//     #region Targeting
//     /// <summary>
//     /// 1. Raycast from screen center. Floor hits are used directly; walls, roofs and sky
//     ///    fall back to a ground search.
//     /// 2. Clamp to max range.
//     /// 3. Push the point away from nearby walls so the footprint doesn't overlap them.
//     /// 4. Check the ceiling above the final point, clamping the spawn height.
//     /// </summary>
//     private bool TryGetTarget(out TargetInfo target)
//     {
//         target = default;
//
//         if (!PlayerSkillHandlerHelper.HasAimContext(_context))
//             return false;
//
//         Ray ray = PlayerSkillHandlerHelper.GetCameraRay(_context);
//         Vector3 characterPos = _context.CharacterTransform.position;
//
//         Vector3 point = default;
//         Vector3 normal = Vector3.up;
//         Vector3 aimPoint;
//         Vector3 searchOrigin = default;
//         bool needsGroundSearch;
//
//         if (Physics.Raycast(ray, out RaycastHit hit, AimDistance, _summonLayer, QueryTriggerInteraction.Ignore))
//         {
//             aimPoint = hit.point;
//
//             if (IsGround(hit.normal))
//             {
//                 point = hit.point;
//                 normal = hit.normal;
//                 needsGroundSearch = false;
//             }
//             else
//             {
//                 // Wall or roof: drop to the floor beside / below the surface.
//                 // Walls: start just in front of the wall. Roofs: start just under the roof.
//                 searchOrigin = hit.normal.y < 0f
//                     ? hit.point + Vector3.down * 0.1f
//                     : hit.point + hit.normal * 0.1f;
//                 needsGroundSearch = true;
//             }
//         }
//         else
//         {
//             // Sky / nothing hit: use the far end of the aim ray.
//             aimPoint = ray.origin + ray.direction * AimDistance;
//             searchOrigin = GetDefaultSearchOrigin(aimPoint, characterPos.y);
//             needsGroundSearch = true;
//         }
//
//         // Clamp to max range (horizontal distance from the character).
//         Vector3 flat = aimPoint - characterPos;
//         flat.y = 0f;
//         if (flat.magnitude > _range)
//         {
//             Vector3 clamped = characterPos + flat.normalized * _range;
//             searchOrigin = GetDefaultSearchOrigin(new Vector3(clamped.x, aimPoint.y, clamped.z), characterPos.y);
//             needsGroundSearch = true;
//         }
//
//         if (needsGroundSearch && !FindGroundBelow(searchOrigin, out point, out normal))
//             return false;
//
//         if (_clearanceRadius > 0f && !ResolveWallOverlap(ref point, ref normal))
//             return false;
//
//         if (!TryGetSpawnHeight(point, out float spawnHeight))
//             return false;
//
//         target = new TargetInfo { Point = point, Normal = normal, SpawnHeight = spawnHeight };
//         return true;
//     }
//
//     /// <summary>
//     /// Start height for a ground search above a mid-air point: high enough to clear
//     /// uphill terrain, capped so it doesn't start above indoor ceilings.
//     /// </summary>
//     private Vector3 GetDefaultSearchOrigin(Vector3 position, float characterY)
//     {
//         float startY = Mathf.Clamp(position.y, characterY + 1f, characterY + _groundSearchUp);
//         return new Vector3(position.x, startY, position.z);
//     }
//
//     /// <summary>
//     /// Drops a ray straight down from the origin onto the floor.
//     /// </summary>
//     private bool FindGroundBelow(Vector3 origin, out Vector3 point, out Vector3 normal)
//     {
//         point = default;
//         normal = Vector3.up;
//
//         if (!Physics.Raycast(origin, Vector3.down, out RaycastHit groundHit,
//                 _groundSearchUp + _groundSearchDown, _summonLayer, QueryTriggerInteraction.Ignore))
//             return false;
//
//         if (!IsGround(groundHit.normal))
//             return false;
//
//         point = groundHit.point;
//         normal = groundHit.normal;
//         return true;
//     }
//
//     private bool IsGround(Vector3 normal)
//         => PlayerSkillHandlerHelper.IsGround(normal, _maxGroundAngle);
//
//     #endregion
//
//     #region Wall / Ceiling Clearance
//
//     /// <summary>
//     /// Iteratively pushes the point out of walls (deepest penetration first),
//     /// re-projecting onto the floor each time. Fails if it can't fit.
//     /// </summary>
//     private bool ResolveWallOverlap(ref Vector3 point, ref Vector3 normal)
//     {
//         for (int i = 0; i < _resolveIterations; i++)
//         {
//             if (!ProbeWalls(point, out Vector3 pushDir, out float penetration))
//                 return true; // clear
//
//             Vector3 moved = point + pushDir * penetration;
//
//             if (!ReprojectToGround(moved, point.y, out Vector3 groundPoint, out Vector3 groundNormal))
//                 return false; // pushed off a ledge / into a void
//
//             point = groundPoint;
//             normal = groundNormal;
//         }
//
//         // Final verification: still overlapping (e.g. corridor narrower than the footprint) -> invalid.
//         return !ProbeWalls(point, out _, out float remaining) || remaining < 0.05f;
//     }
//
//     /// <summary>
//     /// Fires a ring of horizontal rays from the point. Returns the deepest wall penetration
//     /// and the horizontal direction to move to get out of it.
//     /// </summary>
//     private bool ProbeWalls(Vector3 point, out Vector3 pushDir, out float penetration)
//     {
//         pushDir = Vector3.zero;
//         penetration = 0f;
//
//         Vector3 origin = point + Vector3.up * _probeHeight;
//         float step = 360f / _probeCount;
//
//         for (int i = 0; i < _probeCount; i++)
//         {
//             Vector3 dir = Quaternion.AngleAxis(step * i, Vector3.up) * Vector3.forward;
//
//             if (!Physics.Raycast(origin, dir, out RaycastHit wallHit, _clearanceRadius,
//                     _summonLayer, QueryTriggerInteraction.Ignore))
//                 continue;
//
//             // Slopes / ramps are floor, not walls.
//             if (IsGround(wallHit.normal))
//                 continue;
//
//             Vector3 flatNormal = Vector3.ProjectOnPlane(wallHit.normal, Vector3.up);
//             if (flatNormal.sqrMagnitude < 0.0001f)
//                 continue; // roof/overhang underside, handled by the ceiling check
//             flatNormal.Normalize();
//
//             // Perpendicular distance from the point to the wall plane.
//             float perpendicular = wallHit.distance * Mathf.Max(0f, Vector3.Dot(-dir, flatNormal));
//             float pen = _clearanceRadius - perpendicular;
//
//             if (pen > penetration)
//             {
//                 penetration = pen;
//                 pushDir = flatNormal;
//             }
//         }
//
//         return penetration > 0.001f;
//     }
//
//     private bool ReprojectToGround(Vector3 position, float referenceY, out Vector3 point, out Vector3 normal)
//     {
//         point = default;
//         normal = Vector3.up;
//
//         Vector3 origin = new Vector3(position.x, referenceY + 0.5f, position.z);
//
//         if (!Physics.Raycast(origin, Vector3.down, out RaycastHit groundHit, 3f,
//                 _summonLayer, QueryTriggerInteraction.Ignore))
//             return false;
//
//         if (!IsGround(groundHit.normal))
//             return false;
//
//         point = groundHit.point;
//         normal = groundHit.normal;
//         return true;
//     }
//
//     /// <summary>
//     /// Sphere-casts upward so a roof edge next to the point is also respected,
//     /// then clamps the spawn height below it.
//     /// </summary>
//     private bool TryGetSpawnHeight(Vector3 point, out float spawnHeight)
//     {
//         spawnHeight = _spawnHeight;
//
//         float radius = Mathf.Max(0.05f, _clearanceRadius);
//         float startOffset = radius + 0.05f; // start just above the floor so the sphere doesn't touch it
//         Vector3 origin = point + Vector3.up * startOffset;
//         float castDistance = _spawnHeight - startOffset;
//
//         if (castDistance > 0f &&
//             Physics.SphereCast(origin, radius, Vector3.up, out RaycastHit ceiling, castDistance,
//                 _summonLayer, QueryTriggerInteraction.Ignore))
//         {
//             // Sphere center travelled ceiling.distance before touching the roof.
//             spawnHeight = startOffset + ceiling.distance - _ceilingMargin;
//             if (spawnHeight < _minSpawnHeight)
//                 return false;
//         }
//
//         return true;
//     }
//
//     #endregion
// }