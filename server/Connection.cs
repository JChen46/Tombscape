using SpacetimeDB;
using StdbModule;

public static partial class Module
{
    [Reducer(ReducerKind.ClientConnected)]
    public static void Connect(ReducerContext ctx)
    {
        // DoConnect(ctx);
    }

    [Reducer]
    public static void DoConnect(ReducerContext ctx)
    {
        var player = ctx.Db.Logged_out_player.Identity.Find(ctx.Sender);
        if (player != null)
        {
            Log.Debug($"User connected: {ctx.Sender}");
            ctx.Db.Player.Insert(player.Value);
            ctx.Db.Logged_out_player.Identity.Delete(player.Value.Identity);
        }
        else
        {
            Log.Debug($"New User connected: {ctx.Sender}");
            ctx.Db.Player.Insert(new Player
            {
                Identity = ctx.Sender,
                Name = ""
            });
        }
    }

    [Reducer(ReducerKind.ClientDisconnected)]
    public static void Disconnect(ReducerContext ctx)
    {
        // DoDisconnect(ctx);
    }

    [Reducer]
    public static void DoDisconnect(ReducerContext ctx)
    {
        Log.Debug($"User disconnected: {ctx.Sender}");
        var player = ctx.Db.Player.Identity.Find(ctx.Sender) ?? throw new Exception("Player not found");
        var character = ctx.Db.Character.PlayerId.Find(player.PlayerId) ?? throw new Exception("Character not found");
        var entity = ctx.Db.Entity.EntityId.Find(character.EntityId) ?? throw new Exception("Entity not found");
        ctx.Db.Player.Delete(player);
        ctx.Db.Character.Delete(character);
        ctx.Db.Entity.Delete(entity);
        ctx.Db.Logged_out_player.Insert(player);
    }

    [Reducer]
    public static void EnterGame(ReducerContext ctx, string name)
    {
        Log.Info($"Creating player with name {name}");
        var player = ctx.Db.Player.Identity.Find(ctx.Sender) ?? throw new Exception("Player not found");
        player.Name = name;
        ctx.Db.Player.Identity.Update(player);
        var entity = ctx.Db.Entity.Insert(new Entity
        {
            Position = new DbVector2(0, 0)
        });
        var character = ctx.Db.Character.Insert(new Character
        {
            EntityId = entity.EntityId,
            PlayerId = player.PlayerId,
            Health = StartingHealth
        });
        Log.Info($"Player {name} entered");
    }

    [Reducer]
    public static void CreateDummyPlayer(ReducerContext ctx)
    {
        Log.Info("Creating dummy");
        var hexString = Util.GenerateUniqueHexString(64);
        Log.Debug($"Random string: {hexString}");
        Identity dummyIdentity = Identity.FromHexString(hexString); // needs to be 64 to be a hex string for U256 data structure, else 32
        var entity = ctx.Db.Entity.Insert(new Entity
        {
            Position = new DbVector2(10, 10)
        });
        var player = ctx.Db.Player.Insert(new Player
        {
            Identity = dummyIdentity,
            Name = "dummy-" + new Random().Next(1, 100)
        });
        ctx.Db.Character.Insert(new Character
        {
            EntityId = entity.EntityId,
            PlayerId = player.PlayerId,
            Health = 100000
        });
        Log.Info($"Create dummy with identity {player.Identity}");
    }

    [Reducer]
    public static void TestSingle(ReducerContext ctx, string targetName)
    {
        var targetPlayer = ctx.Db.Player.Name.Filter(targetName).SingleOrDefault();
        Log.Info($"Testing singleordefault: {targetPlayer.ToString()}");
    }
}