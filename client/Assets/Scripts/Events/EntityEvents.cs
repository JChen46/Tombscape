using System;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;
using UnityEngine.Events;

public static class EntityEvents
{
    public static UnityEvent<Vector3Int> OnEntityClicked = new UnityEvent<Vector3Int>();
    public static UnityEvent<uint> OnEntityDeleted = new UnityEvent<uint>();

    public static void RaiseEntityClicked(Vector3Int tilePos) => OnEntityClicked?.Invoke(tilePos);
    public static void RaiseEntityDeleted(uint entityId) => OnEntityDeleted?.Invoke(entityId);
    
}