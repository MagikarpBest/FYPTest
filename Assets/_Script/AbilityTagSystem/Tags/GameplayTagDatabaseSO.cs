using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameplayTagDatabase", menuName = "AbilitySystem/GameplayTagDatabase")]
public class GameplayTagDatabaseSO : ScriptableObject
{
    [SerializeField] private List<string> tags = new List<string>();
    public IReadOnlyList<string> Tags => tags;
}
