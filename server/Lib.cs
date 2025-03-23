using SpacetimeDB;
using Index = SpacetimeDB.Index;

public static partial class Module
{
    [Table(Name = "config", Public = true)]
    public partial struct Config
    {
        [PrimaryKey] public uint id;
        public ulong world_size;
    }
    
    [Table(Name = "entity", Public = true)]
    public partial struct Entity
    {
        [PrimaryKey, AutoInc] 
        public uint entity_id;
        public DbVector2 position;
    }
    
    [Table(Name = "player", Public = true)]
    [Table(Name = "logged_out_player")]
    public partial struct Player
    {
        [PrimaryKey]
        public Identity identity;
        [Unique, AutoInc]
        public uint player_id;
        public string name;
    }

    [Table(Name = "character", Public = true)]
    public partial struct Character
    {
        [PrimaryKey] public uint entity_id;
        [Unique] public uint player_id;
    }

    [Table(Name = "action", Public = true)]
    public partial struct Action
    {
        [PrimaryKey, AutoInc] public uint action_id;
        [Index.BTree] public ulong tick_id;
        public string type;
    }

    [Table(Name = "movement_action", Public = true)]
    public partial struct MovementAction
    {
        [PrimaryKey] public uint action_id;
        [Unique] public uint player_id;
        public DbVector2 position;
    }

    [Table(Name = "tick_timer", Scheduled = nameof(EndTick), ScheduledAt = nameof(scheduled_at))]
    public partial struct TickTimer
    {
        [PrimaryKey, AutoInc] public ulong scheduled_id;
        [Unique, AutoInc] public ulong tick_id; // Need to be different from scheduled_id?
        public ScheduleAt scheduled_at;
        [Index.BTree] public bool ended;
    }
    
    
    
    // ================================ Reducers ================================
    
    [Reducer(ReducerKind.Init)]
    public static void Init(ReducerContext ctx)
    {
        Log.Info($"Initializing...");
        ctx.Db.config.Insert(new Config { world_size = DEFAULT_WORLD_SIZE });
        ctx.Db.tick_timer.Insert(new TickTimer
        {
            scheduled_at = new ScheduleAt.Interval(TimeSpan.FromMilliseconds(600))
        });
    }

    [Reducer]
    public static void Test(ReducerContext ctx)
    {
        Log.Info($"Testing...");
        Log.Info(ctx.ToString());
    }

    [Reducer]
    public static void TestUpdate(ReducerContext ctx, String name)
    {
        Log.Info($"TestUpdate ::  with name: {name}");
        var player = ctx.Db.player.identity.Find(ctx.Sender) ?? throw new Exception("Player not found");
        player.name = name;
        Log.Info($"TestUpdate :: player {ctx.Sender} updated to {name}");
        ctx.Db.player.identity.Update(player);
    }

    [Reducer]
    public static void EndTick(ReducerContext ctx, TickTimer tick)
    {
        Log.Info($"EndTick...");
        tick.ended = true;
        ctx.Db.tick_timer.tick_id.Update(tick);
        foreach (var action in ctx.Db.action.tick_id.Filter(tick.tick_id))
        {
            switch (action.type)
            {
                case "movement":
                    DoMovementAction(ctx, action);
                    break;
            }
        }
    }

    [Reducer]
    public static void DoMovementAction(ReducerContext ctx, Action action)
    {
        var movementAction = ctx.Db.movement_action.action_id.Find(action.action_id);
        if (movementAction.HasValue)
        {
            var player = ctx.Db.player.player_id.Find(movementAction.Value.player_id) ?? throw new Exception("Player not found");
            var pc = ctx.Db.character.player_id.Find(player.player_id) ?? throw new Exception("Player not found");
            var entity = ctx.Db.entity.entity_id.Find(pc.entity_id) ?? throw new Exception("Entity not found");
            var distance = movementAction.Value.position - entity.position;
            if (Math.Abs(distance.x) <= 2 && Math.Abs(distance.y) <= 2)
            {
                Log.Info($"Moving to {movementAction.Value.position.x}, {movementAction.Value.position.y}");
                entity.position = movementAction.Value.position;
                ctx.Db.entity.entity_id.Update(entity);
            }
        }
    }

    [Reducer]
    public static void CreateMovementAction(ReducerContext ctx, int x, int y)
    {
        var tick = ctx.Db.tick_timer.ended.Filter(true).First();
        var player = ctx.Db.player.identity.Find(ctx.Sender) ?? throw new Exception("Player not found");
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
