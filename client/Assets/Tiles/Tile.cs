using UnityEngine;

public abstract class Tile : MonoBehaviour
{

    [SerializeField] protected SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private bool _isWalkable;

    public BaseUnit OccupiedUnit;
    public string TileName;
    public bool Walkable => _isWalkable && OccupiedUnit == null;

    public int x;
    public int y;

    public virtual void Init(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    void OnMouseEnter()
    {
        _highlight.SetActive(true);
        MenuManager.Instance.ShowTileInfo(this);
    }
    void OnMouseExit()
    {
        _highlight.SetActive(false);
        MenuManager.Instance.ShowTileInfo(null);
    }

    void OnMouseDown()
    {
        UnitManager.Instance.MoveToTile(this);
        // if (GameManager.Instance.GameState != GameState.HeroesTurn) return;
        //
        // if (OccupiedUnit != null)
        // {
        //     if(OccupiedUnit.Faction == Faction.Hero) UnitManager.Instance.MoveToTile((BaseHero) OccupiedUnit);
        //     else
        //     {
        //         // if user clicks on enemy, delete enemy
        //         if (UnitManager.Instance.SelectedHero != null)
        //         {
        //             var enemy = (BaseEnemy)OccupiedUnit;
        //             Destroy(enemy.gameObject);
        //             UnitManager.Instance.MoveToTile(null);
        //         }
        //     }
        // }
        // else // select hero
        // {
        //     if (UnitManager.Instance.SelectedHero != null && this._isWalkable)
        //     {
        //         SetUnit(UnitManager.Instance.SelectedHero); // move and deselect unit
        //         UnitManager.Instance.MoveToTile(null);
        //     }
        // }
    }

    public void SetUnit(BaseUnit unit)
    {
        if (unit.OccupiedTile != null) unit.OccupiedTile.OccupiedUnit = null;
        unit.transform.position = transform.position;
        OccupiedUnit = unit;
        unit.OccupiedTile = this;
    }
}
