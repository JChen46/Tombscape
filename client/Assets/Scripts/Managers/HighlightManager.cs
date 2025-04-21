using UnityEngine;
using UnityEngine.Tilemaps;

public class HighlightManager : MonoBehaviour
{
    [SerializeField] public Tilemap tilemap;
    [SerializeField] public GameObject highlighter;

    void Start()
    {
        Debug.Log("Starting highlight manager");
        highlighter.SetActive(true);
        highlighter.transform.position = new Vector3(0, 0, 0);
    }
    void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = tilemap.WorldToCell(mouseWorldPos);

        if (tilemap.HasTile(cellPos))
        {
            // Debug.Log($"Found tile  {cellPos}");
            highlighter.SetActive(true);
            Vector3 highlightPos = tilemap.GetCellCenterWorld(cellPos);
            highlighter.transform.position = highlightPos;
            if (Input.GetMouseButtonDown(0)) // if LMB pressed
            {
                Debug.Log($"Clicked on tile {cellPos}");
            }
        }
        else
        {
            highlighter.SetActive(false);
        }
    }
}