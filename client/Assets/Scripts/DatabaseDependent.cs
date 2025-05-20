using SpacetimeDB.Types;
using UnityEngine;
using Util;

public class DatabaseDependent : MonoBehaviour
{
    [SerializeField] protected DatabaseMediator databaseMediator;

    protected RemoteTables Db => databaseMediator.Conn.Db;
    public OneShotEvent OnDataReady { get; } = new(); // TODO: implement this
}