using UnityEngine;
using UnityEngine.Tilemaps;

public class EntityListener : MonoBehaviour
{
    [SerializeField] public Tilemap tilemap;
    void OnEnable()
    {
        EntityEvents.OnEntityClicked += HandleEntityClicked;
    }

    void OnDisable()
    {
        EntityEvents.OnEntityClicked -= HandleEntityClicked;
    }

    void HandleEntityClicked(Vector3Int tilePos)
    {
        
    }
}
