using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SocketEntry
{
    public GameplayTag tag;
    public Transform transform;
}

public class Sockets : MonoBehaviour
{
    [SerializeField] private List<SocketEntry> entries = new List<SocketEntry>();

    public bool TryGetSocket(GameplayTag tag, out Transform socket)
    {
        foreach (var entry in entries)
        {
            if (entry.tag.Equals(tag))
            {
                socket = entry.transform;
                return true;
            }
        }

        socket = null;
        return false;
    }

    public bool TryGetSocket(string tag, out Transform socket) => TryGetSocket(new GameplayTag(tag), out socket);
}