using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseManager : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private LayerMask entityLayer;
    [SerializeField] private LayerMask tilemapLayer;

    private static readonly Vector3Int MIN_VECTOR3INT = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
    private Vector3Int lastHoveredTile = MIN_VECTOR3INT;
    private Collider2D lastHoveredEntity;

    void unhoverTile()
    {
        // Unhover tile
        TileEvents.RaiseTileExit();
        lastHoveredTile = MIN_VECTOR3INT;
    }
    
    void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Check for entityLayer collision
        RaycastHit2D entityOnMouse = Physics2D.Raycast(mouseWorldPos, Vector2.zero, 0f, entityLayer);

        if (entityOnMouse.collider)
        {
            // Checks if hovered collider is new
            if (entityOnMouse.collider != lastHoveredEntity)
            {
                // Debug.Log($"Hovered entity: {entityOnMouse.collider.name}");
                // TODO: Handle entity hover interaction
                unhoverTile();
                // Sets entity as last hovered
                lastHoveredEntity = entityOnMouse.collider;
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                // TODO: Handle entity click interaction
            }
        }
        else
        {
            // check for tilemap collision
            RaycastHit2D tilemapOnMouse = Physics2D.Raycast(mouseWorldPos, Vector2.zero, 0f, tilemapLayer);
            if (tilemapOnMouse.collider)
            {
                Vector3Int tilePos = tilemap.WorldToCell(mouseWorldPos);
                
                // Checks if hovered tilemap is new
                if (tilePos != lastHoveredTile)
                {
                    // Debug.Log($"Hit tilemap: {tilemapOnMouse.collider.name}");
                    TileEvents.RaiseTileHovered(tilePos);
                    lastHoveredTile = tilePos;
                } 
                
                if (Input.GetMouseButtonDown(0))
                {
                    // Handle tile clicked interaction
                    TileEvents.RaiseTileClicked(tilePos);
                }
            } else if (lastHoveredTile !=  MIN_VECTOR3INT)
            {
                unhoverTile();
            }
            
        }
        // Reset entity hover if there’s no entity under the mouse
        if (!entityOnMouse.collider && lastHoveredEntity)
        {
            lastHoveredEntity = null;
        }
    }
}