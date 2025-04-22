using System;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterComponent : MonoBehaviour
{
    [SerializeField] private DatabaseMediator databaseMediator;
    
    private PlayerComponent _playerComponent;
    public Character Character;
    
    void Start()
    {
        _playerComponent = GetComponent<PlayerComponent>();
        databaseMediator.WhenConnected(() =>
        {
            Character = databaseMediator.Conn.Db.Character.PlayerId.Find(_playerComponent.PlayerId) ?? throw new Exception("Player not found");
        });
    }
}
