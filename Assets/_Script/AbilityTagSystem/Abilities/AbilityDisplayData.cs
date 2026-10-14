using UnityEngine;

[CreateAssetMenu(fileName = "AbilityDisplayData",menuName = "AbilitySystem/Abilities/Display Data")]
public class AbilityDisplayDataSO : ScriptableObject
{
    [Header("UI Info")]
    public string AbilityName;
    public Sprite Icon;
    public float ManaCost = 0f;
    [TextArea]
    public string description;

    [SerializeField] private AbilityDefinitionSO ability;
    public AbilityDefinitionSO Ability => ability;

}
