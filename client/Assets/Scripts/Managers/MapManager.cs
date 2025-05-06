using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [SerializeField] private Tilemap map;

    // private Dictionary<TileBase, TileData> tileDictionary;

    // TODO: Get spawnable tiles function
    
    // private void Awake()
    // {
    //     tileDictionary = new Dictionary<TileBase, TileData>();
    //
    //     foreach (var tileData in tileDatas)
    //     {
    //         Debug.Log($"Tiledata: {tileData}");
    //         // foreach (var tile in tileData.tiles)
    //         // {
    //         //     tileDictionary.Add(tile, tileData);
    //         // }
    //     }
    // }

    // public float GetTileWalkingSpeed(Vector2 worldPosition)
    // {
    //     Vector3Int gridPosition = map.WorldToCell(worldPosition);
    //
    //     TileBase tile = map.GetTile(gridPosition);
    //
    //     if (tile == null)
    //         return 1f;
    //
    //     float walkingSpeed = tileDictionary[tile].walkingSpeed;
    //
    //     return walkingSpeed;
    // }


    // public TileData GetTileData(Vector3Int tilePosition)
    // {
    //     TileBase tile = map.GetTile(tilePosition);
    //
    //     if (tile == null)
    //         return null;
    //     else
    //         return tileDictionary[tile];
    // }
}