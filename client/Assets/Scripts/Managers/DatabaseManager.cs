using System;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    const string SERVER_URL = "http://127.0.0.1:3000";
    const string MODULE_NAME = "tombscape";
    
    public static Identity LocalIdentity { get; private set; }
    public static DbConnection Conn { get; private set; }

    public void BuildConnection()
    {
        // In order to build a connection to SpacetimeDB we need to register
        // our callbacks and specify a SpacetimeDB server URI and module name.
        var builder = DbConnection.Builder()
                .OnConnect(HandleConnect)
                .OnConnectError(HandleConnectError)
                .OnDisconnect(HandleDisconnect)
                .WithUri(SERVER_URL)
                .WithModuleName(MODULE_NAME);
        
            // If the user has a SpacetimeDB auth token stored in the Unity PlayerPrefs,
            // we can use it to authenticate the connection.
            if (AuthToken.Token != "")
        {
            builder = builder.WithToken(AuthToken.Token);
        }
        
        // Building the connection will establish a connection to the SpacetimeDB
        // server.
        Conn = builder.Build(); 
    }
    
    // Called when we connect to SpacetimeDB and receive our client identity
    void HandleConnect(DbConnection conn, Identity identity, string token)
    {
        Debug.Log("Connected.");
        AuthToken.SaveToken(token);
        LocalIdentity = identity;
    
        // Request all tables
        Conn.SubscriptionBuilder()
            .OnApplied(HandleSubscriptionApplied)
            .SubscribeToAllTables();
        
        // ChangeState(GameState.GenerateGrid);
        // GridManager.RegisterHandler();
    }
    
    void HandleConnectError(Exception ex)
    {
        Debug.LogError($"Connection error: {ex}");
    }
    
    void HandleDisconnect(DbConnection _conn, Exception ex)
    {
        Debug.Log("Disconnected.");
        if (ex != null)
        {
            Debug.LogException(ex);
        }
    }
    
    private void HandleSubscriptionApplied(SubscriptionEventContext ctx)
    {
        Debug.Log("Subscription applied!");
        
        // temporary manual connection while Connect is commented out in server side
        Debug.Log("Calling DoConnect");
        ctx.Reducers.DoConnect();
        
        // Call enter game with the player name
        Debug.Log("Entering game as testPlayer");
        ctx.Reducers.EnterGame("testPlayer");
    }
    
    // public void ChangeState(GameState newState)
    // {
    //     Debug.Log("Entering ChangeState with state: " + newState);
    //     GameState = newState;
    //     switch (newState)
    //     {
    //         case GameState.GenerateGrid:
    //             GridManager.Instance.GenerateGrid();
    //             break;
    //         case GameState.SpawnHeroes:
    //             UnitManager.Instance.SpawnHeroes();
    //             break;
    //         case GameState.SpawnEnemies:
    //             UnitManager.Instance.SpawnEnemies();
    //             break;
    //         case GameState.HeroesTurn:
    //             break;
    //         case GameState.EnemiesTurn:
    //             break;
    //         default:
    //             throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
    //     }
    // }
}
