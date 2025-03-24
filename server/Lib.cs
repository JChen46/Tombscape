using SpacetimeDB;

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
        public bool isMoving;
        public DbVector2 destinationPos;
    }

    [Table(Name = "npc", Public = true)]
    public partial struct Npc
    {
        // [PrimaryKey, AutoInc] public uint entity_id;
        // [SpacetimeDB.Index.BTree] public uint npc_id;
        [PrimaryKey] public uint npc_id;
        [SpacetimeDB.Index.BTree] public uint entity_id;
        [SpacetimeDB.Index.BTree] public uint player_id;

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
    
    // Random range util functions
    public static float Range(this Random rng, float min, float max) => rng.NextSingle() * (max - min) + min;
    public static int Range(this Random rng, int min, int max) => (int)rng.NextInt64(min, max);
    
    // ================================ Reducers ================================
    
    [Reducer(ReducerKind.Init)]
    public static void Init(ReducerContext ctx)
    {
        Log.Info($"Initializing...");
        ctx.Db.config.Insert(new Config { world_size = DEFAULT_WORLD_SIZE });
        
        // Call schedulers
        ctx.Db.move_all_players_timer.Insert(new MoveAllPlayersTimer
        {
            scheduled_at = new ScheduleAt.Interval(TimeSpan.FromMilliseconds(1000))
        });
    }

    [Reducer]
    public static void Test(ReducerContext ctx)
    {
        Log.Info($"Testing...");
        Log.Info(ctx.ToString());
    }

    [Reducer]
    public static void TestConnect(ReducerContext ctx)
    {
        Log.Info($"TestConnect called...");
        Connect(ctx);
        Log.Info($"TestConnect finished.");
    }

    [Reducer]
    public static void TestUpdate(ReducerContext ctx, String name)
    {
        Log.Info($"TestUpdate :: user {ctx.Sender} updating with name: {name}");
        var player = ctx.Db.player.identity.Find(ctx.Sender) ?? throw new Exception("Player not found");
        player.name = name;
        Log.Info($"TestUpdate :: user {ctx.Sender} name updated to {name}");
        ctx.Db.player.identity.Update(player);
    }

    [Reducer]
    public static void TestMovePlayer(ReducerContext ctx, int x, int y)
    {
        var player = ctx.Db.player.identity.Find(ctx.Sender) ?? throw new Exception("Player not found");
        Log.Info($"Setting player {player.name} destination coords to {x},{y}");
        var player_npc = ctx.Db.npc.player_id.Filter(player.player_id) ?? throw new Exception("Player not found");
        var player_entity = ctx.Db.entity.entity_id.Find(player_npc.FirstOrDefault().entity_id) ?? throw new Exception("Player not found");
        player_entity.destinationPos =  new DbVector2(x, y);
        ctx.Db.entity.entity_id.Update(player_entity);
        Log.Info($"Set player_entity {player_entity.entity_id} to x: {x}, y: {y}");
    }
}
