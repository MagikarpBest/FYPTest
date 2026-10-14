using UnityEditor;
using UnityEngine;

/// <summary>
/// Place all the decoration info here. Additional info gathered from handler
/// </summary>
[CreateAssetMenu(fileName = "NewPlayerSkill", menuName = "Skills/PlayerSkill")]
public class PlayerSkill : ScriptableObject
{
    public string SkillName;
    public Sprite Icon;
    public float ManaCost;
    public PlayerActionRestrictions Restrictions;
    [SerializeReference]
    private IPlayerSkillHandler _handler;
    public IPlayerSkillHandler Handler => _handler;
}
