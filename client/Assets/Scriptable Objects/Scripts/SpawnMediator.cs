using System.Collections.Generic;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnMediator", menuName = "Scriptable Objects/SpawnMediator")]
public class SpawnMediator : ScriptableObject
{
    [SerializeField] private GameObject playerPrefab;

    // Dictionary with entityId key, game object value
    private readonly Dictionary<uint, GameObject> _players = new();
    
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
            
            _players.Add(entity.EntityId, playerGameObject);
        }
        else
        {
            Debug.LogWarning("Player prefab missing PlayerData script.");
        }
    }

    public void DeletePlayer(uint entityId)
    {
        if (_players.TryGetValue(entityId, out var gameObjectToBeDeleted))
        {
            Destroy(gameObjectToBeDeleted);
        }
        _players.Remove(entityId);
        
    }
}
