using System;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] DatabaseManager databaseManager;

    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        Debug.Log("Starting GameManager...");
        Application.targetFrameRate = 60;
        databaseManager.BuildConnection();
    }
}
