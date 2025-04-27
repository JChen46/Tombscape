using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseManager : MonoBehaviour
{
    public Tilemap tilemap;

    private Vector3Int lastHoveredTile;

    void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPos = tilemap.WorldToCell(mouseWorldPos);

        if (Input.GetMouseButtonDown(0)) // if LMB pressed
        {
            TileEvents.RaiseTileClicked(gridPos);
        }
        
        if (gridPos != lastHoveredTile) // TODO: figure out how to only highlight one thing at a time
        {
            if (tilemap.HasTile(gridPos))
            {
                // Debug.Log("Raising tile hovered");
                TileEvents.RaiseTileHovered(gridPos);

                lastHoveredTile = gridPos;
            }
            else if(tilemap.HasTile(lastHoveredTile))
            {
                // Debug.Log("Raising tile exited");
                TileEvents.RaiseTileExit(gridPos);
                lastHoveredTile = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
            }
        }
    }
}