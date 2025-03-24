using SpacetimeDB;

public static partial class Module
{
    private const uint MovementSpeed = 2;

    [Table(Name = "movement_action", Public = true)]
    public partial struct MovementAction
    {
        [PrimaryKey] public uint action_id;
        [Unique] public uint player_id;
        public DbVector2 position;
    }
    
    [Reducer]
    public static void DoMovementAction(ReducerContext ctx, Tick tick, Action action)
    {
        var movementAction = ctx.Db.movement_action.action_id.Find(action.action_id);
        if (movementAction.HasValue)
        {
            var player = ctx.Db.player.player_id.Find(movementAction.Value.player_id) ?? throw new Exception("Player not found");
            var character = ctx.Db.character.player_id.Find(player.player_id) ?? throw new Exception("Player not found");
            var entity = ctx.Db.entity.entity_id.Find(character.entity_id) ?? throw new Exception("Entity not found");
            var newPosition = MoveTowards(entity.position, movementAction.Value.position, MovementSpeed);
            Log.Info($"Moving {player.name} to {newPosition.x}, {newPosition.y}");
            if (newPosition == movementAction.Value.position)
            {
                Log.Info($"Finishing moving");
                ctx.Db.movement_action.Delete(movementAction.Value);
                ctx.Db.action.Delete(action);
            }
            entity.position = newPosition;
            ctx.Db.entity.entity_id.Update(entity);
        }
    }

    private static DbVector2 MoveTowards(DbVector2 current, DbVector2 target, uint speed)
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
        return MoveTowards(current, target, speed);
    }

    [Reducer]
    public static void CreateMovementAction(ReducerContext ctx, int x, int y)
    {
        var tick = ctx.Db.tick.Iter().Reverse().First();
        var player = ctx.Db.player.identity.Find(ctx.Sender) ?? throw new Exception("Player not found");
        var existingAction = ctx.Db.movement_action.player_id.Find(player.player_id);
        if (existingAction.HasValue)
        {
            ctx.Db.action.action_id.Delete(existingAction.Value.action_id);
            ctx.Db.movement_action.Delete(existingAction.Value);
        }
        var action = ctx.Db.action.Insert(new Action
        {
            tick_id = tick.tick_id,
            type = "movement",
        });
        ctx.Db.movement_action.Insert(new MovementAction
        {
            action_id = action.action_id,
            position = new DbVector2(x, y),
            player_id = player.player_id,
        });
        Log.Info($"Created movement action {action.action_id} to {x}, {y} with tick {tick.tick_id}");
    }
}