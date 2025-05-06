using SpacetimeDB.Types;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerData : MonoBehaviour
{
    public PlayerInfo playerInfo;
    [SerializeField] private GameObject highlighter;
    [SerializeField] private DatabaseMediator databaseMediator;
    private Tilemap _tilemap;
    private RemoteTables Db => databaseMediator.Conn.Db;

    public void Init(PlayerInfo info)
    {
        // Clone so each player has their own instance
        playerInfo = Instantiate(info);

        Debug.Log($"Player spawned: {playerInfo.playerName} (ID: {playerInfo.playerId})");
    }

    private void Awake()
    {
        // get tilemap from Tilemap tag
        _tilemap = GameObject.FindGameObjectWithTag("Tilemap").GetComponent<Tilemap>();
    }
    
    private void Start() 
    {
        databaseMediator.WhenConnected(() =>
        {
            Db.Entity.OnUpdate += (context, row, newRow) =>
            {
                Debug.Log($"Updating position of entity {newRow.EntityId} to {newRow.Position.X}, {newRow.Position.Y}");
                if (playerInfo.EntityId == newRow.EntityId)
                {
                    var newPos = _tilemap.CellToWorld(new Vector3Int(newRow.Position.X, newRow.Position.Y));
                    Debug.Log($"Before transform: {transform.position}");
                    transform.position = new Vector3(newPos.x + 0.5f, newPos.y + 0.5f, transform.position.z);
                    Debug.Log($"After transform: {transform.position}");
                }
                else
                {
                    Debug.LogError($"playerInfo entityId: {playerInfo.EntityId} doesn't match newRow EntityId: {newRow.EntityId}");
                }
            };
        });
    }

    void OnMouseEnter()
    {
        highlighter.SetActive(true);
    }

    void OnMouseExit()
    {
        highlighter.SetActive(false);
    }
}

// TODO: visual key input system prototype
// TODO: color visual line attacks + backend logic functionality runnable
// TODO: target highlight/selection