using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnMediator", menuName = "Scriptable Objects/SpawnMediator")]
public class SpawnMediator : ScriptableObject
{
    [SerializeField] private GameObject playerPrefab;


    public void SpawnPlayer(Vector3 spawnPosition, Identity playerIdentity, Player player, Entity entity)
    {
        spawnPosition += new Vector3(0.5f, 0.5f, 0); // offset players

        PlayerData spawnedPlayer =
            Instantiate(playerPrefab, spawnPosition, Quaternion.identity).GetComponent<PlayerData>();

        if (spawnedPlayer != null)
        {
            PlayerInfo playerInfo = ScriptableObject.CreateInstance<PlayerInfo>();
            playerInfo.identity = playerIdentity;
            playerInfo.playerId = player.PlayerId;
            playerInfo.playerName = player.Name;
            playerInfo.EntityId = entity.EntityId;
            playerInfo.Position = Vector3Int.FloorToInt(spawnPosition); // Caution: may have unintended side-effects
            spawnedPlayer.Init(playerInfo);
        }
        else
        {
            Debug.LogWarning("Player prefab missing PlayerData script.");
        }
    }
}
