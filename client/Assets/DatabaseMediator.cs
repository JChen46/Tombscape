using System;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;

[CreateAssetMenu(fileName = "Database Mediator", menuName = "Scriptable Objects/Database")]
public class DatabaseMediator : ScriptableObject
{
    [SerializeField] public string connectionString;
    [SerializeField] public string moduleName;
    
    public Identity LocalIdentity { get; private set; }
    public DbConnection Conn { get; private set; }

    private bool _connected;

    public delegate void OnConnectHandler();
    private event OnConnectHandler OnConnect;

    public void Connect()
    {
        _connected = false;
        // In order to build a connection to SpacetimeDB we need to register
        // our callbacks and specify a SpacetimeDB server URI and module name.
        var builder = DbConnection.Builder()
            .OnConnect(DoHandleConnect)
            .OnConnectError(HandleConnectError)
            .OnDisconnect(HandleDisconnect)
            .WithUri(connectionString)
            .WithModuleName(moduleName);

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

    public void WhenConnected(OnConnectHandler handler)
    {
        if (_connected)
        {
            handler();
        }
        else
        {
            OnConnect += handler;
        }
    }

    public void Disconnect()
    {
        Conn?.Disconnect();
        _connected = false;
    }
    
    // Called when we connect to SpacetimeDB and receive our client identity
    private void DoHandleConnect(DbConnection conn, Identity identity, string token)
    {
        Debug.Log("Connected.");
        AuthToken.SaveToken(token);
        LocalIdentity = identity;

        // Request all tables
        Conn.SubscriptionBuilder()
            .OnApplied(HandleSubscriptionApplied)
            .SubscribeToAllTables();

        Conn.Db.Player.OnInsert += (context, row) =>
        {
            Log.Info("Inserted.");
        };
        Conn.Db.Entity.OnInsert += (context, row) => 
        {
            Log.Info("Entity Inserted.");
        };
    }
    
    private void HandleConnectError(Exception ex)
    {
        Debug.LogError($"Connection error: {ex}");
    }
    
    private void HandleDisconnect(DbConnection conn, Exception ex)
    {
        _connected = false;
        Debug.Log("Disconnected.");
        if (ex != null)
        {
            Debug.LogException(ex);
        }
    }
    
    private void HandleSubscriptionApplied(SubscriptionEventContext ctx)
    {
        Debug.Log("Subscription applied!");
        
        _connected = true;
        OnConnect?.Invoke();

        Conn.Db.Player.OnInsert += (context, row) =>
        {
            Log.Info("Inserted.");
        };
    }
}
