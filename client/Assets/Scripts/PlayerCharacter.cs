using System;
using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;
using UnityEngine.Serialization;
using Util;

public class PlayerCharacter : DatabaseDependent, IHasEntity
{
    public GameManager gameManager;

    private Player _player;
    public Entity Entity { get; private set; }

    private void Start()
    {
        gameManager.OnEnter.Subscribe(() =>
        {
            _player = Db.Player.Identity.Find(databaseMediator.LocalIdentity) ?? throw new Exception("Player not found");
            Log.Info($"Found player {_player.PlayerId}");
            var character = Db.Character.PlayerId.Find(_player.PlayerId) ?? throw new Exception($"Player not found {_player.PlayerId}");
            Entity = Db.Entity.EntityId.Find(character.EntityId) ?? throw new Exception($"Entity not found {character.EntityId}");
            OnDataReady.Invoke();
        });
    }
}
