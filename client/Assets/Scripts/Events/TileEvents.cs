using System;
using UnityEngine;

public static class TileEvents
{
    public static event Action<Vector3Int> OnTileHovered;
    public static event Action<Vector3Int> OnTileClicked;
    public static event Action<Vector3Int> OnTileExit;

    public static void RaiseTileHovered(Vector3Int tilePos) => OnTileHovered?.Invoke(tilePos);
    
    public static void RaiseTileClicked(Vector3Int tilePos) => OnTileClicked?.Invoke(tilePos);
    public static void RaiseTileExit(Vector3Int tilePos) => OnTileExit?.Invoke(tilePos);
}