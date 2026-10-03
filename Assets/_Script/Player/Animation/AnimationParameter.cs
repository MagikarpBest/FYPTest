using UnityEngine;

public static class AnimationParameter
{
    public static readonly int AttackTrigger1 =
        Animator.StringToHash("AttackTrigger1");

    public static readonly int AttackTrigger2 =
        Animator.StringToHash("AttackTrigger2");

    public static readonly int AttackTrigger3 =
        Animator.StringToHash("AttackTrigger3");

    public static readonly int IsComboQueued = 
        Animator.StringToHash("IsComboQueued");
}
