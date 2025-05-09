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
    public readonly Dictionary<uint, GameObject> entitiesDict = new();
    
    public void SpawnPlayer(Vector3Int spawnPosition, Identity playerIdentity, Player player, Entity entity)
    {

        GameObject playerGameObject = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        PlayerData spawnedPlayer = playerGameObject.GetComponent<PlayerData>();

        if (spawnedPlayer != null)
        {
            spawnedPlayer.Init(player, entity);
            
            entitiesDict.Add(entity.EntityId, playerGameObject);
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
        if (entitiesDict.TryGetValue(entityId, out var gameObjectToBeDeleted))
        {
            Destroy(gameObjectToBeDeleted);
        }
        entitiesDict.Remove(entityId);
        
    }
}
