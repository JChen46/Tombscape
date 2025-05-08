using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TileStateData
{
    [FormerlySerializedAs("tileType")] public TileBase tileBase;
    public string tileName;
    
    public TileStateData(TileBase tileBase)
    {
        this.tileBase = tileBase;
        this.tileName = "DEFAULT_TILE_NAME";
    }
}