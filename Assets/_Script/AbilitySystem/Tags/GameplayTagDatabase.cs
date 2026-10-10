using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameplayTagDatabase", menuName = "Scriptable Objects/GameplayTagDatabase")]
public class GameplayTagDatabaseSO : ScriptableObject
{
    [SerializeField] private List<string> tags = new List<string>();
    public IReadOnlyList<string> Tags => tags;
}

public static class GameplayTagDatabase
{
    private static GameplayTagDatabaseSO tagDatabase;
    private static List<GameplayTag> tags;
    public static IReadOnlyList<GameplayTag> Tags => tags;
    public static void Init(GameplayTagDatabaseSO database)
    {
        if (database == null)
        {
            Debug.LogError("Init gameplay database is null");
            return;
        }
        
        tagDatabase = database;
        tags = new List<GameplayTag>();
        
        foreach (var tag in tagDatabase.Tags)
        {
            tags.Add(new GameplayTag(tag));
        }
    }
    
    public static bool Contains(GameplayTag tag)
    {
        return tags.Contains(tag);
    }
}

