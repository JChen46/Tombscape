using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    private List<ScriptableUnit> _units;

    public BaseHero SelectedHero;

    void Awake()
    {
        Instance = this;

        _units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
    }

    public void SpawnHeroes()
    {
        var heroCount = 1; // hard coded for now
        for (int i = 0; i < heroCount; i++)
        {
            var randomPrefab = GetRandomUnit<BaseHero>(Faction.Hero);
            var spawnedHero = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.Instance.GetHeroSpawnTile();

            randomSpawnTile.SetUnit(spawnedHero);
            Debug.Log("Spawned Hero: " + spawnedHero.name);
            SelectedHero = spawnedHero;
        }
    }
    public void SpawnEnemies()
    {
        var enemyCount = 1; // hard coded for now
        for (int i = 0; i < enemyCount; i++)
        {
            var randomPrefab = GetRandomUnit<BaseEnemy>(Faction.Enemy);
            var spawnedEnemy = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();

            randomSpawnTile.SetUnit(spawnedEnemy);
            Debug.Log("Spawned Enemy: " + spawnedEnemy.name);
        }

    }

    private T GetRandomUnit<T>(Faction faction) where T : BaseUnit
    {
        // goes through all units, finds the ones that match the faction, randomizes the order, then returns the first one's prefab
        return (T)_units.Where(u => u.Faction == faction).OrderBy(o => Random.value).First().UnitPrefab;
    }

    public void MoveToTile(Tile tile)
    {
        // GameManager.Conn.Reducers.CreateMovementAction((int)tile.transform.position.x, (int)tile.transform.position.y);
    }
    
}
