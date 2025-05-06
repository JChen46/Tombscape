using System;
using System.Collections.Generic;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;

public class OtherCharacterManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private DatabaseMediator databaseMediator;
    [SerializeField] private OtherPlayerCharacter otherCharacterPrefab;
    
    private readonly Dictionary<uint, OtherPlayerCharacter> _characters = new();

    private void Start()
    {
        gameManager.OnEnter.Subscribe(() =>
        {
            foreach (var character in databaseMediator.Conn.Db.Character.Iter())
            {
                AddCharacter(character);
            }
            databaseMediator.Conn.Db.Character.OnInsert += (context, row) =>
            {
                if (!_characters.ContainsKey(row.EntityId))
                {
                    AddCharacter(row);
                }
            };
            databaseMediator.Conn.Db.Character.OnDelete += (context, row) =>
            {
                Log.Info($"Removing character {row.EntityId}");
                _characters.TryGetValue(row.EntityId, out var value);
                if (value) Destroy(value.gameObject);
                _characters.Remove(row.EntityId);
            };
        });
    }

    private void AddCharacter(Character character)
    {
        Log.Info($"Adding character {character.EntityId}");
        var player = databaseMediator.Conn.Db.Player.PlayerId.Find(character.PlayerId) ?? throw new Exception("Player not found");
        if (player.Identity == databaseMediator.LocalIdentity)
        {
            return;
        }
        var entity = databaseMediator.Conn.Db.Entity.EntityId.Find(character.EntityId) ?? throw new Exception("Entity not found");
        var newCharacter = Instantiate(otherCharacterPrefab, Vector3.zero, Quaternion.identity);
        newCharacter.Entity = entity;
        newCharacter.Player = player;
        newCharacter.gameObject.SetActive(true);
        _characters.Add(character.EntityId, newCharacter);
    }
}