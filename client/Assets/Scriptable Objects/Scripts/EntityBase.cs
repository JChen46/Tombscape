using UnityEngine;

[CreateAssetMenu(fileName = "EntityBase", menuName = "Scriptable Objects/EntityBase")]
public class EntityBase : ScriptableObject
{
    public uint EntityId;
    public Vector3Int Position;
}
