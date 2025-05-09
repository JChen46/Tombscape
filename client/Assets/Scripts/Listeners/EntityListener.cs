using SpacetimeDB.Types;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EntityListener : MonoBehaviour
{
    [SerializeField] public Tilemap tilemap;
    [SerializeField] public SpawnMediator spawnMediator;
    [SerializeField] private DatabaseMediator databaseMediator;
    
    private RemoteTables Db => databaseMediator.Conn.Db;
    
    private void Start() 
    {
        // TODO: Fork in the road - either handle subscription events by manager (searches for entity to update and updates it) vs entity subscribing to updates (entities check on every update to see if they're updated)
        // This current implementation is the former
        databaseMediator.OnConnect.Subscribe(() =>
        {
            Db.Entity.OnUpdate += (context, row, newRow) =>
            {
                // Anytime an entity gets updated, move that entity to its new position
                Debug.Log($"Updating position of entity {newRow.EntityId} to {newRow.Position.X}, {newRow.Position.Y}");
                var newPos = tilemap.CellToWorld(new Vector3Int(newRow.Position.X, newRow.Position.Y));
                if (spawnMediator.entitiesDict.TryGetValue(newRow.EntityId, out var entityToMove))
                {
                    entityToMove.transform.position = new Vector3(newPos.x + 0.5f, newPos.y + 0.5f, transform.position.z);
                }
                else
                {
                    Debug.LogError($"Attempt to move Entity {newRow.EntityId} not found");
                }
                
            };
        });
    }
    void OnEnable()
    {
        EntityEvents.OnEntityClicked.AddListener(HandleEntityClicked);
        EntityEvents.OnEntityDeleted.AddListener(HandleEntityDeleted);
    }

    void OnDisable()
    {
        EntityEvents.OnEntityClicked.RemoveListener(HandleEntityClicked);
        EntityEvents.OnEntityDeleted.RemoveListener(HandleEntityDeleted);
    }

    void HandleEntityClicked(Vector3Int tilePos)
    {
        
    }
    
    void HandleEntityDeleted(uint entityId)
    {
        Debug.Log($"Entity {entityId} deleted.");
        spawnMediator.DeleteEntity(entityId);
    }
}
