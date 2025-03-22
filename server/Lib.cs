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
    }

    [Table(Name = "npc", Public = true)]
    public partial struct Npc
    {
        [PrimaryKey, AutoInc] public uint entity_id;
        [SpacetimeDB.Index.BTree] public uint npc_id;
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
    
    
    // ================================ Reducers ================================
    
    [Reducer(ReducerKind.Init)]
    public static void Init(ReducerContext ctx)
    {
        Log.Info($"Initializing...");
        ctx.Db.config.Insert(new Config { world_size = DEFAULT_WORLD_SIZE });
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
}
