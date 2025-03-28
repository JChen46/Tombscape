using SpacetimeDB;
using Index = SpacetimeDB.Index;

public static partial class Module
{
    private static readonly TimeSpan TickRate = TimeSpan.FromMilliseconds(1800);
    private const float StartingHealth = 100;

    [Table(Name = "Config", Public = true)]
    public partial struct Config
    {
        [PrimaryKey] public uint Id;
        public ulong WorldSize;
    }

    [Table(Name = "Entity", Public = true)]
    public partial struct Entity
    {
        [PrimaryKey, AutoInc] public uint EntityId;
        public DbVector2 Position;
    }

    [Table(Name = "Player", Public = true)]
    [Table(Name = "Logged_out_player")]
    public partial struct Player
    {
        [PrimaryKey] public Identity Identity;
        [Unique, AutoInc] public uint PlayerId;
        [Index.BTree] public string Name;
    }

    [Table(Name = "Character", Public = true)]
    public partial struct Character
    {
        [PrimaryKey] public uint EntityId;
        [Unique] public uint PlayerId;
        public float Health;
    }

    [Table(Name = "Tick", Scheduled = nameof(EndTick), ScheduledAt = nameof(ScheduleAt))]
    public partial struct Tick
    {
        [PrimaryKey, AutoInc] public ulong ScheduledId;
        public ScheduleAt ScheduleAt;
        public Timestamp EndTime;
    }


    // ================================ Reducers ================================

    [Reducer(ReducerKind.Init)]
    public static void Init(ReducerContext ctx)
    {
        Log.Info($"Initializing...");
        ctx.Db.Config.Insert(new Config { WorldSize = DEFAULT_WORLD_SIZE });
        var now = ctx.Timestamp;
        ctx.Db.Tick.Insert(new Tick
        {
            ScheduleAt = new ScheduleAt.Time(now),
            EndTime = now
        });
    }

    [Reducer]
    public static void Test(ReducerContext ctx)
    {
        Log.Info($"Testing...");
        Log.Info(ctx.ToString());
    }

    [Reducer]
    public static void TestUpdate(ReducerContext ctx, string name)
    {
        Log.Info($"TestUpdate ::  with name: {name}");
        var player = ctx.Db.Player.Identity.Find(ctx.Sender) ?? throw new Exception("Player not found");
        player.Name = name;
        Log.Info($"TestUpdate :: player {ctx.Sender} updated to {name}");
        ctx.Db.Player.Identity.Update(player);
    }

    [Reducer]
    public static void EndTick(ReducerContext ctx, Tick tick)
    {
        try
        {
            var nextTick = Timestamp.FromTimeSpanSinceUnixEpoch(tick.EndTime.ToTimeSpanSinceUnixEpoch().Add(TickRate));
            Log.Debug($"EndTick {tick.ScheduledId}, time diff: {nextTick.TimeDurationSince(tick.EndTime)}");
            ctx.Db.Tick.Insert(new Tick
            {
                ScheduleAt = new ScheduleAt.Time(nextTick),
                EndTime = nextTick
            });
            foreach (var movementAction in ctx.Db.Movement_action.Iter()) DoMovementAction(ctx, tick, movementAction);
            foreach (var colorSpellAction in ctx.Db.Color_spell_action.Iter()) DoColorSpellAction(ctx, colorSpellAction);
            foreach (var colorSpellAction in ctx.Db.Color_spell_action.Iter()) ApplyColorSpell(ctx, colorSpellAction);
        }
        catch (Exception e)
        {
            Log.Error($"Error occurred in EndTick: {e.Message}");
        }
    }
}