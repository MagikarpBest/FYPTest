using System;
using UnityEngine;

[Serializable]
public class BombSkillHandler : IPlayerSkillHandler
{
    private PlayerSkillContext _context;
    private Bomb _activeBomb;

    // Data defined by ScriptableObject.
    [SerializeField] private float _range = 10f;
    [SerializeField] private float _explosionRadius = 5f;
    [SerializeField] private float _explosionForce = 5f;
    [SerializeField] private float _explodeDelay = 1.5f;
    [SerializeField] private Bomb _bombPrefab;
    [SerializeField] private float _throwForce = 15f;
    [SerializeField] private float _upwardForce = 5f;
    // Data defined by ScriptableObject.

    #region IPlayerSkillHandler
    public void Begin(PlayerSkillContext context)
    {
        _context = context;
        if (_bombPrefab == null)
            return;
        if (_context.SkillHoldPoint == null)
            return;

 
        _activeBomb = UnityEngine.Object.Instantiate(_bombPrefab, _context.SkillHoldPoint.position, _context.SkillHoldPoint.rotation);
        _activeBomb.Hold(_context.SkillHoldPoint);
    }

    public void Update()
    {
        if (_activeBomb == null)
            return;

        // TODO: Update bomb aiming / direction.
    }

    public SkillConfirmResult Confirm()
    {
        if (_activeBomb == null)
            return SkillConfirmResult.Fail;

        Vector3 throwDirection = _context.Camera.transform.forward;
        if (throwDirection.sqrMagnitude <= 0.0001f)
            return SkillConfirmResult.Fail;

        RotateCharacterToAim(throwDirection);


        _activeBomb.Throw(throwDirection, _throwForce, _upwardForce, _explodeDelay);
        _activeBomb = null;

        return SkillConfirmResult.Finish;
    }

    public void Cancel()
    {
        if (_activeBomb == null)
            return;

        UnityEngine.Object.Destroy(_activeBomb.gameObject);

        _activeBomb = null;
    }
    #endregion

    private void RotateCharacterToAim(Vector3 aimDirection)
    {
        if (_context.CharacterTransform == null)
            return;

        // Only rotate the character horizontally.
        Vector3 flatDirection = new Vector3(aimDirection.x, 0f, aimDirection.z);

        if (flatDirection.sqrMagnitude <= 0.0001f)
            return;

        _context.CharacterTransform.rotation =
            Quaternion.LookRotation(
                flatDirection.normalized,
                Vector3.up);
    }
    
    // state restriction test, change to manageable by scriptable object
    public PlayerActionRestrictions GetRestrictions()
    {
        // stop jumping while placing a pillar
        return PlayerActionRestrictions.RestrictJump;
    }
}