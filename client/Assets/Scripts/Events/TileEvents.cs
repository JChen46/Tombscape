using System;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;
using UnityEngine.Events;

public static class TileEvents
{
    // TODO: Change from C# events to unity events for better support
    public static UnityEvent<Vector3Int> OnTileHovered = new UnityEvent<Vector3Int>();
    public static UnityEvent<Vector3Int> OnTileClicked = new UnityEvent<Vector3Int>();
    public static UnityEvent OnTileExit = new UnityEvent();

    public static void RaiseTileHovered(Vector3Int tilePos) => OnTileHovered?.Invoke(tilePos);
    
    public static void RaiseTileClicked(Vector3Int tilePos) => OnTileClicked?.Invoke(tilePos);
    public static void RaiseTileExit() => OnTileExit?.Invoke();
    
}