using System;
using System.Collections;
using System.Collections.Generic;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField] private DatabaseMediator databaseMediator;

    private bool _entered = false;
    public delegate void OnEnterHandler();
    private event OnEnterHandler OnEnter;

    private void Start()
    {
        Instance = this;
        databaseMediator.Connect();
        databaseMediator.WhenConnected(() =>
        {
            Log.Info("Adding handlers");
            databaseMediator.Conn.Reducers.EnterGame("testPlayer");
            databaseMediator.Conn.Reducers.OnEnterGame += (context, row) =>
            {
                _entered = true;
                OnEnter?.Invoke();
            };
        });
    }

    public void WhenEntered(OnEnterHandler handler)
    {
        if (_entered)
        {
            handler();
        }
        else
        {
            OnEnter += handler;
        }
    }

    void OnDestroy()
    {
        Log.Info("disconnecting");
        databaseMediator.Disconnect();
    }
}