using SpacetimeDB.Types;
using UnityEngine;
using UnityEngine.Tilemaps;
using Util;

public class EntityData : MonoBehaviour, IHasEntity
{

    public Entity Entity { get; private set; }
    public OneShotEvent OnDataReady { get; }

    public void Init(Entity entity)
    {
        Entity = entity;

        Debug.Log($"Entity spawned: {entity.EntityId} at {entity.Position.X}, {entity.Position.Y}");
    }
}