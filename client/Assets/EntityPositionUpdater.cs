using SpacetimeDB.Types;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class EntityPositionUpdater : MonoBehaviour
{
    [SerializeField] private DatabaseMediator databaseMediator;
    private RemoteTables Db => databaseMediator.Conn.Db;

    private Character _character;
    private Tilemap _tilemap;

    private void Start()
    {
        _character = GetComponent<CharacterComponent>().Character;
        _tilemap = GetComponentInParent<Tilemap>();
        databaseMediator.WhenConnected(() =>
        {
            Db.Entity.OnUpdate += (context, row, newRow) =>
            {
                if (_character.EntityId == newRow.EntityId)
                {
                    transform.position = _tilemap.CellToWorld(new Vector3Int(newRow.Position.X, newRow.Position.Y));
                }
            };
        });
    }
}
