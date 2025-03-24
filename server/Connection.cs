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
            var playerEntry = ctx.Db.player.Insert(new Player
            {
                identity = ctx.Sender,
                name = "",
            });
            //create entity table values
            var entityEntry = ctx.Db.entity.Insert(new Entity
            {
                position = new DbVector2(1, 1),
                isMoving = false,
                destinationPos = new DbVector2(1, 1) // TODO: figure out default destinationPos, should be prevented via isMoving?
            });
            // create npc table values
            ctx.Db.npc.Insert(new Npc
            {
                // TODO: establish pre-populated lookup table for npc_ids
                npc_id = 100, // temp hardcoding
                entity_id = entityEntry.entity_id,
                player_id = playerEntry.player_id
            });
            
            Log.Debug($"Created player entry with identity  {playerEntry.identity}");
            Log.Debug($"Created entity entry with entity_id  {entityEntry.entity_id}");
            Log.Debug($"Created npc entry with entity_id  {entityEntry.entity_id} and player_id  {playerEntry.player_id}");
        }
    }

    [Reducer(ReducerKind.ClientDisconnected)]
    public static void Disconnect(ReducerContext ctx)
    {
        Log.Debug($"User disconnected: {ctx.Sender}");
        var player = ctx.Db.player.identity.Find(ctx.Sender) ?? throw new Exception("Player not found");
        // Remove any npc from the arena
        // foreach (var npc in ctx.Db.npc.player_id.Filter(player.player_id))
        // {
        //     var entity = ctx.Db.entity.entity_id.Find(npc.entity_id) ?? throw new Exception($"Could not find npc with entity_id: {npc.entity_id}");
        //     ctx.Db.entity.entity_id.Delete(entity.entity_id);
        //     ctx.Db.npc.entity_id.Delete(entity.entity_id);
        // }
        ctx.Db.logged_out_player.Insert(player);
        ctx.Db.player.identity.Delete(player.identity);
    }
    
    // Currently  unused
    // [Reducer]
    // public static void EnterGame(ReducerContext ctx, string name)
    // {
    //     Log.Info($"Creating player with name {name}");
    //     var player = ctx.Db.player.identity.Find(ctx.Sender) ?? throw new Exception("Player not found");
    //     player.name = name;
    //     ctx.Db.player.identity.Update(player);
    // }
}