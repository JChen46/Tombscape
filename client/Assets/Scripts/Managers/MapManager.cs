using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;

    private Dictionary<Vector3Int, TileStateData> tileDictionary; // Consider changing this dictionary to be of type <TileBase, TileData>? Otherwise <Vector3Int, TileBase> is more simple
    
    void Start()
    {
        tileDictionary = new Dictionary<Vector3Int, TileStateData>();
        if (tilemap == null)
        {
            Debug.LogError("Tilemap reference not set!");
            return;
        }

        BoundsInt bounds = tilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                for (int z = bounds.zMin; z < bounds.zMax; z++) // currently only 1 z index
                {
                    Vector3Int pos = new Vector3Int(x, y, z);
                    TileBase tile = tilemap.GetTile(pos);
                    if (tile != null)
                    {
                        tileDictionary[pos] = new TileStateData(tile);
                    }
                }
            }
        }

        Debug.Log($"Total tiles stored: {tileDictionary.Count}");
    }

    public KeyValuePair<Vector3Int, TileStateData> GetRandomGroundTile()
    {
        // Filter for tiles that are GroundRuleTile and have isWalkable
        var filteredPairs = tileDictionary
            .Where(pair => pair.Value.tileBase is GroundRuleTile { isWalkable: true })
            .ToList();
        
        // Get random one if any exist
        if (filteredPairs.Count > 0)
        {
            var randomPair = filteredPairs[Random.Range(0, filteredPairs.Count)];
            // Vector3Int pos = randomPair.Key;
            // TileBase tile = randomPair.Value;

            return randomPair;
        }

        return default(KeyValuePair<Vector3Int, TileStateData>);
    }

}