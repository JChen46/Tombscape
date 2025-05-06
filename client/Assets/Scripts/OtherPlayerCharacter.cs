
using SpacetimeDB.Types;

public class OtherPlayerCharacter : DatabaseDependent, IHasEntity
{
    public Player Player { get; set; }
    private Entity _entity;
    public Entity Entity
    {
        get => _entity;
        set
        {
            _entity = value;
            OnDataReady.Invoke();
        }
    }
}