using SpacetimeDB;

public static partial class Module
{
    private const uint MovementSpeed = 2;

    [Table(Name = "Movement_action", Public = true)]
    public partial struct MovementAction
    {
        [PrimaryKey, AutoInc] public uint ActionId;
        [Unique] public uint PlayerId;
        public DbVector2 Destination;
    }

    [Reducer]
    public static void DoMovementAction(ReducerContext ctx, Tick tick, MovementAction movementAction)
    {
        var player = ctx.Db.Player.PlayerId.Find(movementAction.PlayerId) ??
                     throw new Exception("Player not found");
        var character = ctx.Db.Character.PlayerId.Find(player.PlayerId) ?? throw new Exception("Player not found");
        var entity = ctx.Db.Entity.EntityId.Find(character.EntityId) ?? throw new Exception("Entity not found");
        var newPosition = MoveTowards(entity.Position, movementAction.Destination, MovementSpeed);
        Log.Info($"Moving {player.Name} to {newPosition.x}, {newPosition.y}");
        if (newPosition == movementAction.Destination)
        {
            Log.Info($"Finishing moving");
            ctx.Db.Movement_action.Delete(movementAction);
        }

        entity.Position = newPosition;
        ctx.Db.Entity.EntityId.Update(entity);
    }

    private static DbVector2 MoveTowards(DbVector2 current, DbVector2 target, uint speed)
    {
        while (true)
        {
            // No real pathfinding yet, just OSRS-style. Could obviously be optimized, but this is simplest
            var direction = target - current;
            if (Math.Abs(direction.x) <= speed && Math.Abs(direction.y) <= speed)
            {
                return target;
            }

            // Recurse
            if (Math.Abs(direction.x) > 0)
            {
                target.x -= Math.Sign(direction.x);
            }

            if (Math.Abs(direction.y) > 0)
            {
                target.y -= Math.Sign(direction.y);
            }
        }
    }

    [Reducer]
    public static void CreateMovementAction(ReducerContext ctx, int x, int y)
    {
        var tick = ctx.Db.Tick.Iter().Last();
        var player = ctx.Db.Player.Identity.Find(ctx.Sender) ?? throw new Exception("Player not found");
        var existingAction = ctx.Db.Movement_action.PlayerId.Find(player.PlayerId);
        if (existingAction is not null)
        {
            ctx.Db.Movement_action.Delete(existingAction.Value);
        }

        var movementActionInsert = ctx.Db.Movement_action.Insert(new MovementAction
        {
            Destination = new DbVector2(x, y),
            PlayerId = player.PlayerId
        });
        Log.Info($"Created movement action {movementActionInsert.ActionId} to {x}, {y} with tick {tick.ScheduledId}");
    }
}