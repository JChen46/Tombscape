
using SpacetimeDB.Types;
using Util;

public interface IHasEntity
{
    public Entity Entity { get; }
    public OneShotEvent OnDataReady { get; }
}