using SpacetimeDB;

public static partial class Module
{
    [Table(Name = "move_all_players_timer", Scheduled = nameof(MoveAllNpcs), ScheduledAt = nameof(scheduled_at))]
    public partial struct MoveAllPlayersTimer
    {
        [PrimaryKey, AutoInc]
        public ulong scheduled_id;
        public ScheduleAt scheduled_at;
    }
    
    [Reducer]
    public static void MoveAllNpcs(ReducerContext ctx, MoveAllPlayersTimer timer)
    {
        // TODO: create pathfinding algorithm similar to osrs using destination tile and current tile
        Log.Info("Executing MoveAllPlayers pathfinding");
        var world_size = (ctx.Db.config.id.Find(0) ?? throw new Exception("Config not found")).world_size;
        
        foreach (var npc in ctx.Db.npc.Iter())
        {
            // check if entity has been killed
            var check_entity = ctx.Db.entity.entity_id.Find(npc.entity_id);
            if (check_entity == null)
            {
                continue;
            }

            var npc_entity = check_entity.Value;
            
            // basic placeholder movement logic
            if (npc_entity.destinationPos != npc_entity.position)
            {
                if (npc_entity.destinationPos.x != npc_entity.position.x)
                {
                    npc_entity.position.x += npc_entity.destinationPos.x > npc_entity.position.x ? 1 : -1;
                    Log.Debug($"Npc entity_id {npc_entity.entity_id} x coord moved to {npc_entity.position.x}");
                }
                if (npc_entity.destinationPos.y != npc_entity.position.y)
                {
                    npc_entity.position.y += npc_entity.destinationPos.y > npc_entity.position.y ? 1 : -1;
                    Log.Debug($"Npc entity_id {npc_entity.entity_id} y coord moved to {npc_entity.position.y}");
                }
                Log.Debug($"npc destination pos x: {npc_entity.destinationPos.x}, pos y: {npc_entity.destinationPos.y}");
                ctx.Db.entity.entity_id.Update(npc_entity);
            }
        }
        
    }
}