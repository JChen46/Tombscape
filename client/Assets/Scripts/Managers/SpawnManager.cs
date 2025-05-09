using System;
using System.Collections.Generic;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private DatabaseMediator databaseMediator;
    [SerializeField] private MapManager mapManager;
    [SerializeField] private GameManager gameManager;

    // Dictionary with entityId key, game object value
    public readonly Dictionary<uint, GameObject> entitiesDict = new();
    private RemoteTables Db => databaseMediator.Conn.Db;

    public void Start()
    {
        gameManager.OnEnter.Subscribe(() =>
        {
            Debug.Log($"Within OnEnterGame");
            if (databaseMediator.LocalIdentity != null)
            {
                Player player = Db.Player.Identity.Find(databaseMediator.LocalIdentity) ??
                                throw new Exception("OnEnterGame :: Player not found");
                Character character = Db.Character.PlayerId.Find(player.PlayerId) ??
                                      throw new Exception("OnEnterGame :: Character not found");
                Entity entity = Db.Entity.EntityId.Find(character.EntityId) ??
                                throw new Exception("OnEnterGame :: Entity not found");
                Vector3Int position = new Vector3Int(entity.Position.X, entity.Position.Y, 0);
                Debug.Log(
                    $"Calling SpawnPlayer with position: {position}, playerId: {player.PlayerId}, playerName: {player.Name}");
                SpawnPlayer(position, databaseMediator.LocalIdentity, player, entity);
            }
            else
            {
                Debug.LogError("GameManager attempted to search for player before LocalIdentity is set");
            }
        });
    }
    
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
