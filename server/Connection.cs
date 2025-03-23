using SpacetimeDB;

public static partial class Module
{
    [Reducer(ReducerKind.ClientConnected)]
    public static void Connect(ReducerContext ctx)
    {
        var player = ctx.Db.logged_out_player.identity.Find(ctx.Sender);
        if (player != null)
        {
            Log.Debug($"User connected: {ctx.Sender}"); 
            ctx.Db.player.Insert(player.Value);
            ctx.Db.logged_out_player.identity.Delete(player.Value.identity);
        }
        else
        {
            Log.Debug($"New User connected: {ctx.Sender}");
            ctx.Db.player.Insert(new Player
            {
                identity = ctx.Sender,
                name = "",
            });
        }
    }

    [Reducer(ReducerKind.ClientDisconnected)]
    public static void Disconnect(ReducerContext ctx)
    {
        Log.Debug($"User disconnected: {ctx.Sender}");
        // var player = ctx.Db.player.identity.Find(ctx.Sender) ?? throw new Exception("Player not found");
        // var character = ctx.Db.character.player_id.Find(player.player_id) ?? throw new Exception("Character not found");
        // var entity = ctx.Db.entity.entity_id.Find(character.entity_id) ?? throw new Exception("Entity not found");
        // ctx.Db.player.Delete(player);
        // ctx.Db.character.Delete(character);
        // ctx.Db.entity.Delete(entity);
        // ctx.Db.logged_out_player.Insert(player);
    }
    
    [Reducer]
    public static void EnterGame(ReducerContext ctx, string name)
    {
        Log.Info($"Creating player with name {name}");
        var player = ctx.Db.player.identity.Find(ctx.Sender) ?? throw new Exception("Player not found");
        player.name = name;
        ctx.Db.player.identity.Update(player);
        var entity = ctx.Db.entity.Insert(new Entity
        {
            position = new DbVector2(0, 0),
        });
        var character = ctx.Db.character.Insert(new Character
        {
            entity_id = entity.entity_id,
            player_id = player.player_id
        });
        Log.Info($"Player {name} entered");
    }
}