using System;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerComponent : MonoBehaviour
{
    [SerializeField] private DatabaseMediator databaseMediator;
    private RemoteTables Db => databaseMediator.Conn.Db;
    public GameManager gameManager;
    
    public Identity Identity;
    public uint PlayerId;
    
    void Awake()
    {
        gameManager.WhenEntered(() =>
        {
            var player = Db.Player.Identity.Find(databaseMediator.LocalIdentity) ?? throw new Exception("Player not found");
            Identity = player.Identity;
            PlayerId = player.PlayerId;
        });
    }
}
