using System.Collections.Generic;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "SpawnMediator", menuName = "Scriptable Objects/SpawnMediator")]
// TODO: Should this spawn mediator even be a scriptable object? To store the _players dictionary?
public class SpawnMediator : ScriptableObject
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private DatabaseMediator databaseMediator;
    [SerializeField] private MapManager mapManager;

    // Dictionary with entityId key, game object value
    private readonly Dictionary<uint, GameObject> _entities = new();
    
    public void SpawnPlayer(Vector3 spawnPosition, Identity playerIdentity, Player player, Entity entity)
    {
        spawnPosition += new Vector3(0.5f, 0.5f, 0); // offset players

        GameObject playerGameObject = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        PlayerData spawnedPlayer = playerGameObject.GetComponent<PlayerData>();

        if (spawnedPlayer != null)
        {
            PlayerInfo playerInfo = ScriptableObject.CreateInstance<PlayerInfo>();
            playerInfo.identity = playerIdentity;
            playerInfo.playerId = player.PlayerId;
            playerInfo.playerName = player.Name;
            playerInfo.EntityId = entity.EntityId;
            playerInfo.Position = Vector3Int.FloorToInt(spawnPosition); // Caution: may have unintended side-effects
            spawnedPlayer.Init(playerInfo);
            
            _entities.Add(entity.EntityId, playerGameObject);
        }
        else
        {
            Debug.LogWarning("Player prefab missing PlayerData script.");
        }
    }

    public void SpawnDummyPlayer() // temporarily unused
    {
        var randomSpawn = mapManager.GetRandomGroundTile();
        if (!randomSpawn.Value.tileBase)
        {
            Debug.Log("Spawning dummy player");
            databaseMediator.Conn.Reducers.CreateDummyPlayer(randomSpawn.Key.x, randomSpawn.Key.y);
            // Handle entity creation here
        }
    }

    public void DeleteEntity(uint entityId)
    {
        if (_entities.TryGetValue(entityId, out var gameObjectToBeDeleted))
        {
            Destroy(gameObjectToBeDeleted);
        }
        _entities.Remove(entityId);
        
    }
}
