using SpacetimeDB;

public static partial class Module
{
    [Reducer(ReducerKind.ClientConnected)]
    public static void Connect(ReducerContext ctx)
    {
        DoConnect(ctx);
    }

    [Reducer]
    public static void DoConnect(ReducerContext ctx)
    {
        var player = ctx.Db.logged_out_player.Identity.Find(ctx.Sender);
        if (player != null)
        {
            Log.Debug($"User connected: {ctx.Sender}"); 
            ctx.Db.player.Insert(player.Value);
            ctx.Db.logged_out_player.Identity.Delete(player.Value.Identity);
        }
        else
        {
            Log.Debug($"New User connected: {ctx.Sender}");
            ctx.Db.player.Insert(new Player
            {
                Identity = ctx.Sender,
                Name = "",
            });
        }
    }

    [Reducer(ReducerKind.ClientDisconnected)]
    public static void Disconnect(ReducerContext ctx)
    {
        DoDisconnect(ctx);
    }

    [Reducer]
    public static void DoDisconnect(ReducerContext ctx)
    {
        Log.Debug($"User disconnected: {ctx.Sender}");
        var player = ctx.Db.player.Identity.Find(ctx.Sender) ?? throw new Exception("Player not found");
        var character = ctx.Db.character.PlayerId.Find(player.PlayerId) ?? throw new Exception("Character not found");
        var entity = ctx.Db.entity.EntityId.Find(character.EntityId) ?? throw new Exception("Entity not found");
        ctx.Db.player.Delete(player);
        ctx.Db.character.Delete(character);
        ctx.Db.entity.Delete(entity);
        ctx.Db.logged_out_player.Insert(player);
    }
    
    [Reducer]
    public static void EnterGame(ReducerContext ctx, string name)
    {
        Log.Info($"Creating player with name {name}");
        var player = ctx.Db.player.Identity.Find(ctx.Sender) ?? throw new Exception("Player not found");
        player.Name = name;
        ctx.Db.player.Identity.Update(player);
        var entity = ctx.Db.entity.Insert(new Entity
        {
            Position = new DbVector2(0, 0),
        });
        var character = ctx.Db.character.Insert(new Character
        {
            EntityId = entity.EntityId,
            PlayerId = player.PlayerId
        });
        Log.Info($"Player {name} entered");
    }
}