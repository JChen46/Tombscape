using System;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] DatabaseMediator databaseMediator;
    [SerializeField] SpawnMediator spawnMediator;
    private RemoteTables Db => databaseMediator.Conn.Db;
    
    // public delegate void OnEnterHandler();
    // private event OnEnterHandler OnEnter;

    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        Debug.Log("Starting GameManager...");
        Application.targetFrameRate = 60;
        
        // TODO: Figure out how to move this spawning logic elsewhere
        databaseMediator.Connect();
        databaseMediator.WhenConnected(() =>
        {
            // TODO: call spawn dummy reducer for testing purposes
            Log.Info("Adding handlers");
            databaseMediator.Conn.Reducers.EnterGame("testPlayer");
            databaseMediator.Conn.Reducers.OnEnterGame += (context, row) =>
            {
                Debug.Log($"Within OnEnterGame, row: {row}, context: {context.Identity}");
                if (databaseMediator.LocalIdentity != null)
                {
                    Player player = Db.Player.Identity.Find(databaseMediator.LocalIdentity) ?? throw new Exception("OnEnterGame :: Player not found");
                    Character character = Db.Character.PlayerId.Find(player.PlayerId) ?? throw new Exception("OnEnterGame :: Character not found");
                    Entity entity = Db.Entity.EntityId.Find(character.EntityId) ?? throw new Exception("OnEnterGame :: Entity not found");
                    Vector3 position = new Vector3(entity.Position.X, entity.Position.Y, 0);
                    Debug.Log($"Calling SpawnPlayer with position: {position}, playerId: {player.PlayerId}, playerName: {player.Name}");
                    spawnMediator.SpawnPlayer(position, databaseMediator.LocalIdentity, player, entity);
                }
                else
                {
                    Debug.LogError("GameManager attempted to search for player before LocalIdentity is set");
                }
                // OnEnter?.Invoke();

                // SpawnManager.SpawnPlayer();
            };
        });
    }
    
}
