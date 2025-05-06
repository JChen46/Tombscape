using System;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;

public static class EntityEvents
{
    public static event Action<Vector3Int> OnEntityClicked;
    public static event Action<uint> OnEntityDeleted;

    public static void RaiseEntityClicked(Vector3Int tilePos) => OnEntityClicked?.Invoke(tilePos);
    public static void RaiseEntityDeleted(uint entityId) => OnEntityDeleted?.Invoke(entityId);
    
}