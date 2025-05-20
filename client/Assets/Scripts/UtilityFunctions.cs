
using System.Collections.Generic;
using UnityEngine;

public static class UtilityFunctions 
{
    public static Vector3 convertToVector3(Vector2 vector2) // currently unused
    {
        return new Vector3(vector2.x, vector2.y, 0);
    }
}

// Pooling used for performance optimization for frequently created/destroyed game objects
public class ProjectilePool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 10;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        if (pool.Count == 0)
            return null;

        GameObject obj = pool.Dequeue();
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        return obj;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
