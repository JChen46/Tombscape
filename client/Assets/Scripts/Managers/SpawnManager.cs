using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private List<PrefabEntry> prefabList;
    [SerializeField] private DatabaseMediator databaseMediator;
    [SerializeField] private MapManager mapManager;
    [SerializeField] private GameManager gameManager;
    
    // Dictionary with entityId key, game object value
    public readonly Dictionary<uint, GameObject> entitiesDict = new();
    
    // Dictionary with prefab name key, game
    private Dictionary<PrefabType, GameObject> prefabDict = new();
    
    private RemoteTables Db => databaseMediator.Conn.Db;

    public enum PrefabType
    {
        PLAYER_PREFAB,
        OTHER_PLAYER_PREFAB,
    }
    [System.Serializable]
    public struct PrefabEntry
    {
        public PrefabType type;
        public GameObject prefab;
    }
    void Awake()
    {
        // populate prefabDict
        prefabDict = new Dictionary<PrefabType, GameObject>();
        foreach (PrefabEntry entry in prefabList)
        {
                prefabDict[entry.type] = entry.prefab;
        }
    }
    public void Start()
    {
        gameManager.OnEnter.Subscribe(() =>
        {
            Debug.Log($"Within OnEnterGame");
            if (databaseMediator.LocalIdentity != null)
            {
                // Player player = Db.Player.Identity.Find(databaseMediator.LocalIdentity) ??
                //                 throw new Exception("OnEnterGame :: Player not found");
                // Character character = Db.Character.PlayerId.Find(player.PlayerId) ??
                //                       throw new Exception("OnEnterGame :: Character not found");
                // Entity entity = Db.Entity.EntityId.Find(character.EntityId) ??
                //                 throw new Exception("OnEnterGame :: Entity not found");
                // Vector3Int position = new Vector3Int(entity.Position.X, entity.Position.Y, 0);
                // Debug.Log(
                //     $"Calling SpawnPlayer with position: {position}, playerId: {player.PlayerId}, playerName: {player.Name}");
                // SpawnEntity(position, entity, player);
            }
            else
            {
                Debug.LogError("GameManager attempted to search for player before LocalIdentity is set");
            }
            
            // TEMPORARY TESTING CODE: Spawn dummy player on random spawnable tile
            SpawnDummyPlayer();
        });
        
        Db.Character.OnInsert += (context, newRow) =>
        {
            Debug.Log(
                $"Character inserting with entity ID {newRow.EntityId}, player ID {newRow.PlayerId}, health {newRow.Health}"); // TODO: create unity entity representing other players
            
            Character character = Db.Character.PlayerId.Find(newRow.PlayerId) ?? throw new Exception("Character not found");
            Debug.Log($"TEST: Insert character ID: {character.EntityId}, player ID: {character.PlayerId}");
            Player player = Db.Player.PlayerId.Find(character.PlayerId) ?? throw new Exception("Player not found");
            
            Entity entity = Db.Entity.EntityId.Find(newRow.EntityId) ??
                            throw new Exception("OnEntity Insert :: Entity not found");
            Vector3Int position = new Vector3Int(entity.Position.X, entity.Position.Y, 0);
            Debug.Log(
                $"Calling SpawnEntity with position: {position}, playerId: {player?.PlayerId}, playerName: {player?.Name}");
            
            PrefabType entityPrefab = databaseMediator.LocalIdentity == player.Identity ? PrefabType.PLAYER_PREFAB : PrefabType.OTHER_PLAYER_PREFAB;

            SpawnEntity(entityPrefab, position, entity, player);
            
        };
        Db.Entity.OnDelete += (context, deletedRow) =>
        {
            Debug.Log($"Entity ID {deletedRow.EntityId} at position {deletedRow.Position.X}, {deletedRow.Position.Y} deleted");
        };
    }

    private GameObject GetPrefab(PrefabType type)
    {
        return prefabDict.GetValueOrDefault(type);
    }

    private void SpawnEntity(PrefabType prefabType, Vector3Int spawnPosition, Entity entity, Player player = null)
    {
        GameObject chosenPrefab = GetPrefab(prefabType) ?? throw new Exception("Could not get prefab");

        GameObject spawnedGameObject = Instantiate(chosenPrefab, spawnPosition, Quaternion.identity);

        if (prefabType == PrefabType.PLAYER_PREFAB)
        {
            PlayerData spawnedPlayer = spawnedGameObject.GetComponent<PlayerData>();
            spawnedPlayer.Init(player, entity);
            // spawnedPlayer.OnDataReady.Invoke();
        }
        else
        {
            EntityData spawnedEntity = spawnedGameObject.GetComponent<EntityData>();
            spawnedEntity.Init(entity);
            // spawnedEntity.OnDataReady.Invoke();
        }
        entitiesDict.Add(entity.EntityId, spawnedGameObject);
        Debug.Log($"Added entity {entity.EntityId} to  entitiesDict");
        
    }

    private void SpawnDummyPlayer() // for testing purposes
    {
        var randomSpawn = mapManager.GetRandomGroundTile();
        if (randomSpawn.Value.tileBase)
        {
            Debug.Log("Spawning dummy player");
            databaseMediator.Conn.Reducers.CreateDummyPlayer(randomSpawn.Key.x, randomSpawn.Key.y);
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
