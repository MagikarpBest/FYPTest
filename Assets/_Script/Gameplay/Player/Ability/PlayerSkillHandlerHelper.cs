using UnityEngine;

/// <summary>
/// Stateless helpers shared by player skill handlers.
/// Config values are passed in as parameters so the helper never depends on a handler.
/// </summary>
public static class PlayerSkillHandlerHelper
{
    public const float DefaultAimDistance = 100f;
    private static readonly Vector3 ScreenCenter = new Vector3(0.5f, 0.5f, 0f);

    #region Context / Camera

    /// <summary>True when the context has everything needed for aiming.</summary>
    public static bool HasAimContext(PlayerSkillContext context)
    {
        return context != null && context.Camera != null && context.CharacterTransform != null;
    }

    /// <summary>Ray from the center of the screen. Call HasAimContext first.</summary>
    public static Ray GetCameraRay(PlayerSkillContext context)
    {
        return context.Camera.ViewportPointToRay(ScreenCenter);
    }

    /// <summary>Draws the aim line on the context's LineRenderer, if there is one.</summary>
    public static void DrawCameraRay(PlayerSkillContext context, float distance = DefaultAimDistance)
    {
        if (context?.AimLineRenderer == null || context.Camera == null)
            return;

        Ray ray = GetCameraRay(context);
        LineRenderer line = context.AimLineRenderer;

        line.positionCount = 2;
        line.SetPosition(0, ray.origin);
        line.SetPosition(1, ray.origin + ray.direction * distance);
    }

    #endregion

    #region Targeting

    /// <summary>Raycast from screen center. Returns false if the context is invalid or nothing is hit.</summary>
    public static bool TryRaycastFromCamera(PlayerSkillContext context, float distance, LayerMask layer, 
        out RaycastHit hit, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
    {
        hit = default;

        if (!HasAimContext(context))
            return false;

        return Physics.Raycast(GetCameraRay(context), out hit, distance, layer, triggerInteraction);
    }

    /// <summary>Distance from the character to the point is within range.</summary>
    public static bool IsWithinRange(PlayerSkillContext context, Vector3 point, float range)
    {
        if (context == null || context.CharacterTransform == null)
            return false;

        return Vector3.Distance(context.CharacterTransform.position, point) <= range;
    }

    /// <summary>A surface counts as floor if its normal is within maxGroundAngle of up.</summary>
    public static bool IsGround(Vector3 normal, float maxGroundAngle)
    {
        return normal.y >= Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }

    #endregion

    #region Character

    /// <summary>Rotates the character horizontally to face the direction.</summary>
    public static void RotateCharacterToAim(PlayerSkillContext context, Vector3 aimDirection)
    {
        if (context == null || context.CharacterTransform == null)
            return;

        Vector3 flat = new Vector3(aimDirection.x, 0f, aimDirection.z);
        if (flat.sqrMagnitude <= 0.0001f)
            return;

        context.CharacterTransform.rotation = Quaternion.LookRotation(flat.normalized, Vector3.up);
    }

    #endregion

    #region Lifetime

    /// <summary>Destroys the GameObject (if any) and clears the reference.</summary>
    public static void DestroyAndClear(ref GameObject target)
    {
        if (target == null)
            return;

        Object.Destroy(target);
        target = null;
    }

    /// <summary>Destroys the component's whole GameObject (if any) and clears the reference.</summary>
    public static void DestroyAndClear<T>(ref T component) where T : Component
    {
        if (component == null)
            return;

        Object.Destroy(component.gameObject);
        component = null;
    }

    #endregion
}