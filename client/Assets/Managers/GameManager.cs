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

    void Awake()
    {
        Instance = this;
        databaseMediator.Connect();
        databaseMediator.WhenConnected(() =>
        {
            databaseMediator.Conn.Reducers.EnterGame("testPlayer");
        });
    }

    void OnDestroy()
    {
        databaseMediator.Disconnect();
    }
}