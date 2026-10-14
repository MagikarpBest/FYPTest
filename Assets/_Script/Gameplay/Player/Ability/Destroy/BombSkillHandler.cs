using System;
using UnityEngine;

[Serializable]
public class BombSkillHandler : IPlayerSkillHandler
{
    private PlayerSkillContext _context;
    private Bomb _activeBomb;

    // Data defined by ScriptableObject.
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

        if (_bombPrefab == null || _context.SkillHoldPoint == null)
            return;

        _activeBomb = UnityEngine.Object.Instantiate(
            _bombPrefab,
            _context.SkillHoldPoint.position,
            _context.SkillHoldPoint.rotation);
        _activeBomb.Hold(_context.SkillHoldPoint);
        _activeBomb.Init(_explosionRadius, _explosionForce);
    }

    public void Update()
    {
        if (_activeBomb == null)
            return;

        // TODO: Update bomb aiming / direction.
    }

    public SkillConfirmResult Confirm(out ISummonedObject summonedObject)
    {
        summonedObject = null;

        if (_activeBomb == null)
            return SkillConfirmResult.Fail;

        Vector3 throwDirection = _context.Camera.transform.forward;
        if (throwDirection.sqrMagnitude <= 0.0001f)
            return SkillConfirmResult.Fail;

        PlayerSkillHandlerHelper.RotateCharacterToAim(_context, throwDirection);

        summonedObject = _activeBomb.GetComponent<ISummonedObject>();
        _activeBomb.Throw(throwDirection, _throwForce, _upwardForce, _explodeDelay);
        _activeBomb = null;

        return SkillConfirmResult.Finish;
    }

    public void Cancel()
    {
        PlayerSkillHandlerHelper.DestroyAndClear(ref _activeBomb);
    }

    #endregion
}