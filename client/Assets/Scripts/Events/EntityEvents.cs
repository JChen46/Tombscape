using System;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;

public static class EntityEvents
{
    public static event Action<Vector3Int> OnEntityClicked;

    public static void RaiseEntityClicked(Vector3Int tilePos) => OnEntityClicked?.Invoke(tilePos);
    
}