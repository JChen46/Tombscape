using SpacetimeDB.Types;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerData : DatabaseDependent , IHasEntity
{
    [SerializeField] private GameObject highlighter;

    private Player _player;
    public Entity Entity { get; private set; }
    public void Init(Player player, Entity entity)
    {
        _player = player;
        Entity = entity;

        Debug.Log($"Player spawned: {_player.Name} (ID: {_player.PlayerId})");
    }


    // Temporary highlight functionality, should be moved away from OnMouseEnter/OnMouseExit
    void OnMouseEnter()
    {
        highlighter.SetActive(true);
    }

    void OnMouseExit()
    {
        highlighter.SetActive(false);
    }
}

// TODO: visual key input system prototype
// TODO: color visual line attacks + backend logic functionality runnable
// TODO: target highlight/selection