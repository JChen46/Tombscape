using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class EntityPositionUpdater : MonoBehaviour
{
    [SerializeField] private DatabaseMediator databaseMediator;
    private RemoteTables Db => databaseMediator.Conn.Db;
    public GameManager gameManager;

    private CharacterComponent _character;
    public Tilemap tilemap;

    private void Start()
    {
        _character = GetComponent<CharacterComponent>();
        gameManager.WhenEntered(() =>
        {
            Db.Entity.OnUpdate += (context, row, newRow) =>
            {
                Log.Info($"Updating position of entity {newRow.EntityId} to {newRow.Position.X}, {newRow.Position.Y}");
                if (_character.Character.EntityId == newRow.EntityId)
                {
                    var newPos = tilemap.CellToWorld(new Vector3Int(newRow.Position.X, newRow.Position.Y));
                    transform.position = new Vector3(newPos.x + 0.5f, newPos.y + 0.5f, transform.position.z);
                }
            };
        });
    }
}
