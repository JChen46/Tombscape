using System;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterComponent : MonoBehaviour
{
    [SerializeField] private DatabaseMediator databaseMediator;
    public GameManager gameManager;
    
    private PlayerComponent _playerComponent;
    public Character Character;
    
    void Start()
    {
        _playerComponent = GetComponent<PlayerComponent>();
        gameManager.WhenEntered(() =>
        {
            Log.Info("Found character");
            Character = databaseMediator.Conn.Db.Character.PlayerId.Find(_playerComponent.PlayerId) ?? throw new Exception($"Player not found {_playerComponent.PlayerId}");
        });
    }
}
