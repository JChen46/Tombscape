using UnityEngine;
using UnityEngine.Tilemaps;

public class EntityListener : MonoBehaviour
{
    [SerializeField] public Tilemap tilemap;
    [SerializeField] public SpawnMediator spawnMediator;
    void OnEnable()
    {
        EntityEvents.OnEntityClicked.AddListener(HandleEntityClicked);
        EntityEvents.OnEntityDeleted.AddListener(HandleEntityDeleted);
    }

    void OnDisable()
    {
        EntityEvents.OnEntityClicked.RemoveListener(HandleEntityClicked);
        EntityEvents.OnEntityDeleted.RemoveListener(HandleEntityDeleted);
    }

    void HandleEntityClicked(Vector3Int tilePos)
    {
        
    }
    
    void HandleEntityDeleted(uint entityId)
    {
        Debug.Log($"Entity {entityId} deleted.");
        spawnMediator.DeleteEntity(entityId);
    }
}
