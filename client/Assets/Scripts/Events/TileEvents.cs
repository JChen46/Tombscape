using System;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;

public static class TileEvents
{
    // TODO: Change from C# events to unity events for better support
    // public static UnityEvent<Vector3Int> OnTileHovered;
    public static event Action<Vector3Int> OnTileHovered;
    public static event Action<Vector3Int> OnTileClicked;
    public static event Action OnTileExit;

    public static void RaiseTileHovered(Vector3Int tilePos) => OnTileHovered?.Invoke(tilePos);
    
    public static void RaiseTileClicked(Vector3Int tilePos) => OnTileClicked?.Invoke(tilePos);
    public static void RaiseTileExit() => OnTileExit?.Invoke();
    
}