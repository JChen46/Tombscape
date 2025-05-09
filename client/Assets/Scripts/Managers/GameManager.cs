using System;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Util;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] DatabaseMediator databaseMediator;
    [SerializeField] SpawnManager spawnManager;
    private RemoteTables Db => databaseMediator.Conn.Db;
    
    public readonly OneShotEvent OnEnter = new();

    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        Debug.Log("Starting GameManager...");
        Application.targetFrameRate = 60;
        
        databaseMediator.Connect();
        databaseMediator.OnConnect.Subscribe(() =>
        {
            // TODO: call spawn dummy reducer for testing purposes
            Log.Info("Adding handlers");
            databaseMediator.Conn.Reducers.EnterGame("testPlayer");
            databaseMediator.Conn.Reducers.OnEnterGame += (context, enterGameName) =>
            {
                Debug.Log($"OnEnterGame {enterGameName}, context: {context.Identity}");
                OnEnter?.Invoke();
            };
        });
    }

    void OnDestroy()
    {
        Debug.Log("Destroying GameManager...");
    }
    
}
