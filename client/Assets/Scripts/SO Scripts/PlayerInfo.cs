using SpacetimeDB;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInfo", menuName = "Scriptable Objects/PlayerInfo")]
public class PlayerInfo : EntityBase
{
    public Identity identity;
    public uint playerId;
    public string playerName;
}
