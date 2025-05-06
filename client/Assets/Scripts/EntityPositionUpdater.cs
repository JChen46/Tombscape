using SpacetimeDB;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EntityPositionUpdater : DatabaseDependent
{
    public GameManager gameManager;

    private IHasEntity _hasEntity;
    public Tilemap tilemap;

    private void Start()
    {
        _hasEntity = GetComponent<IHasEntity>();
        gameManager.OnEnter.Subscribe(() =>
        {
            Db.Entity.OnUpdate += (context, row, newRow) =>
            {
                Log.Info($"Receiving entity update {newRow.EntityId}");
                _hasEntity.OnDataReady.Subscribe(() =>
                {
                    Log.Info($"Updating position of entity {newRow.EntityId} to {newRow.Position.X}, {newRow.Position.Y}");
                    if (_hasEntity.Entity.EntityId == newRow.EntityId)
                    {
                        var newPos = tilemap.CellToWorld(new Vector3Int(newRow.Position.X, newRow.Position.Y));
                        transform.position = new Vector3(newPos.x + 0.5f, newPos.y + 0.5f, transform.position.z);
                    }
                });
            };
        });
    }
}
