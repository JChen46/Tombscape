using SpacetimeDB;
using Index = SpacetimeDB.Index;

public static partial class Module
{
    private static readonly TimeSpan TickRate = TimeSpan.FromMilliseconds(600);

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

    [Table(Name = "tick", Scheduled = nameof(EndTick), ScheduledAt = nameof(schedule_at))]
    public partial struct Tick
    {
        [PrimaryKey, AutoInc] public ulong scheduled_id;
        [Unique, AutoInc] public ulong tick_id;
        public ScheduleAt schedule_at;
        public Timestamp end_time;
    }
    
    
    
    // ================================ Reducers ================================
    
    [Reducer(ReducerKind.Init)]
    public static void Init(ReducerContext ctx)
    {
        Log.Info($"Initializing...");
        ctx.Db.config.Insert(new Config { world_size = DEFAULT_WORLD_SIZE });
        var now = ctx.Timestamp;
        ctx.Db.tick.Insert(new Tick
        {
            schedule_at = new ScheduleAt.Time(now),
            end_time = now
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
    public static void EndTick(ReducerContext ctx, Tick tick)
    {
        Log.Info($"EndTick {tick.tick_id}");
        var nextTick = Timestamp.FromTimeSpanSinceUnixEpoch(tick.end_time.ToTimeSpanSinceUnixEpoch().Add(TickRate));
        ctx.Db.tick.Insert(new Tick
        {
            schedule_at = new ScheduleAt.Time(nextTick),
            end_time = nextTick,
        });
        foreach (var action in ctx.Db.action.Iter())
        {
            switch (action.type)
            {
                case "movement":
                    DoMovementAction(ctx, tick, action);
                    break;
            }
        }
    }
}
