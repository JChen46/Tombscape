using SpacetimeDB;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ClickComponent : MonoBehaviour
{
    [SerializeField] private DatabaseMediator databaseMediator;
    private Tilemap _tilemap;
    void Start()
    {
        _tilemap = GetComponentInParent<Tilemap>();
    }

    void OnMouseDown()
    {
        var worldToCell = _tilemap.WorldToCell(transform.position);
        Log.Info($"moving to {worldToCell}");
        databaseMediator.OnConnect.Subscribe(() =>
        {
            Log.Info($"creating movement action");
            databaseMediator.Conn.Reducers.CreateMovementAction(worldToCell.x, worldToCell.y);
        });
    }
    
}
