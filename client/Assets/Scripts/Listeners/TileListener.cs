using System;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileListener : MonoBehaviour
{
    [SerializeField] public Tilemap tilemap;
    [SerializeField] public GameObject highlighter;
    [SerializeField] private DatabaseMediator databaseMediator;

    // private Vector3Int? currentTile = null;

    void OnEnable()
    {
        TileEvents.OnTileHovered += HandleTileHovered;
        TileEvents.OnTileClicked += HandleTileClicked;
        TileEvents.OnTileExit += HandleTileExit;
    }

    void OnDisable()
    {
        TileEvents.OnTileHovered -= HandleTileHovered;
        TileEvents.OnTileClicked -= HandleTileClicked;
        TileEvents.OnTileExit -= HandleTileExit;
    }

    void HandleTileHovered(Vector3Int tilePos)
    {
        highlighter.SetActive(true);
        Vector3 highlightPos = tilemap.GetCellCenterWorld(tilePos);
        highlighter.transform.position = highlightPos;
    }
    
    private void HandleTileClicked(Vector3Int tilePos)
    {
        TileBase tile = tilemap.GetTile(tilePos);
        GroundRuleTile groundRuleTile = tile as GroundRuleTile;
        if (groundRuleTile != null)
        {
            Debug.Log($"Clicked tile at {tilePos}; walkable: {groundRuleTile.isWalkable}");
            databaseMediator.Conn.Reducers.CreateMovementAction(tilePos.x, tilePos.y);
        }
        else
        {
            Debug.Log("Tile is not a GroundRuleTile");
        }
        // tilemap.SetTile(tilePos, null); // deletes tile
        // highlighter.SetActive(false); // part of deleting the tile
    }

    void HandleTileExit()
    {
        // Debug.Log("Exiting tile");
        highlighter.SetActive(false);
    }

}
