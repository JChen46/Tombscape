using System;
using SpacetimeDB;
using UnityEngine;
using Util;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField] private DatabaseMediator databaseMediator;

    public readonly OneShotEvent OnEnter = new OneShotEvent();

    private void Start()
    {
        Instance = this;
        databaseMediator.Connect();
        databaseMediator.OnConnect.Subscribe(() =>
        {
            Log.Info("Adding handlers");
            databaseMediator.Conn.Reducers.EnterGame("testPlayer");
            databaseMediator.Conn.Reducers.OnEnterGame += (context, row) =>
            {
                OnEnter.Invoke();
            };
        });
    }

    void OnDestroy()
    {
        Log.Info("disconnecting");
        databaseMediator.Disconnect();
    }
}