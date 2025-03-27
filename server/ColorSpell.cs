using SpacetimeDB;
using StdbModule;

public static partial class Module
{
    [Table(Name = "color_spell_action")]
    public partial struct ColorSpellAction
    {
        [PrimaryKey, AutoInc] public uint ActionId;
        public uint CasterCharacterId;
        public uint TargetCharacterId;
        public ColorSpellType SpellType;
        
        public ColorSpell Spell => BaseColorSpells.SpellSteps[SpellType];
    }
    
    [Table(Name = "color_spell_state")]
    public partial struct ColorSpellState
    {
        [PrimaryKey] public uint ActionId;
        public int SpellStep;
    }
    
    [Reducer]
    public static void DoColorSpellAction(ReducerContext ctx, ColorSpellAction action)
    {
        if (Util.IsNotNull(ctx.Db.color_spell_state.ActionId.Find(action.ActionId), out var state))
        {
            ctx.Db.color_spell_state.Insert(new ColorSpellState
            {
                ActionId = action.ActionId,
                SpellStep = 0
            });
        }
        else
        {
            state.SpellStep += 1;
            var spell = action.Spell;
            if (state.SpellStep == spell.Length)
            {
                ctx.Db.color_spell_state.Delete(state);
                ctx.Db.color_spell_action.Delete(action);
            }
        }
        
    }

    [Reducer]
    public static void ApplyColorSpell(ReducerContext ctx, ColorSpellAction action)
    {
        var state = ctx.Db.color_spell_state.ActionId.Find(action.ActionId) ?? throw new Exception("Spell state not found");
        var spell = action.Spell;
        var target = ctx.Db.character.PlayerId.Find(action.TargetCharacterId) ?? throw new Exception("Target player not found");
        // TODO: Calculate multiplier
        target.Health -= spell.Steps[state.SpellStep].Damage;
    }

    [Reducer]
    public static void CreateColorSpellAction(ReducerContext ctx, string color, string length, string targetName)
    {
        var player = ctx.Db.player.Identity.Find(ctx.Sender) ?? throw new Exception("Player not found");
        var caster = ctx.Db.character.PlayerId.Find(player.PlayerId) ?? throw new Exception("Character not found");
        var targetPlayer = ctx.Db.player.Name.Filter(targetName).SingleOrDefault();
        var target = ctx.Db.character.PlayerId.Find(targetPlayer.PlayerId) ??
                     throw new Exception("Target character not found");
        Enum.TryParse(color, true, out Color inputColor);
        Enum.TryParse(length, true, out Length inputLength);
        ctx.Db.color_spell_action.Insert(new ColorSpellAction
        {
            CasterCharacterId = caster.EntityId,
            TargetCharacterId = target.EntityId,
            SpellType = new ColorSpellType
            {
                Color = inputColor,
                Length = inputLength
            }
        });
        Log.Info($"Color spell action with color {inputColor} and length {inputLength} has been created");
    }
}

[Type]
public partial record ColorSpellType
{
    public required Color Color { get; init; }
    public required Length Length { get; init; }

    public virtual bool Equals(ColorSpellType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Color == other.Color && Length == other.Length;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)Color, (int)Length);
    }
}

[Type]
public enum Color
{
    Red,
    Green,
    Blue
}

[Type]
public enum Length
{
    Short,
    Long
}

public record ColorSpell
{
    public required uint Length { get; init; }
    public required List<ColorSpellStep> Steps { get; init; }
}

public record ColorSpellStep
{
    public required float Damage { get; init; }
    public required Color Color { get; init; }
}

internal static class BaseColorSpells
{
    public static readonly Dictionary<ColorSpellType, ColorSpell> SpellSteps =
        new()
        {
            {
                new ColorSpellType { Color = Color.Red, Length = Length.Short },
                new ColorSpell
                {
                    Length = 2, Steps = [
                        new ColorSpellStep { Damage = 5, Color = Color.Red },
                        new ColorSpellStep { Damage = 0, Color = Color.Red },
                    ]
                }
            },
            {
                new ColorSpellType { Color = Color.Red, Length = Length.Long },
                new ColorSpell
                {
                    Length = 4, Steps = [
                        new ColorSpellStep { Damage = 7, Color = Color.Red },
                        new ColorSpellStep { Damage = 5, Color = Color.Red },
                        new ColorSpellStep { Damage = 2, Color = Color.Red },
                        new ColorSpellStep { Damage = 0, Color = Color.Red },
                    ]
                }
            },
            {
                new ColorSpellType { Color = Color.Green, Length = Length.Short },
                new ColorSpell
                {
                    Length = 3, Steps = [
                        new ColorSpellStep { Damage = 5, Color = Color.Green },
                        new ColorSpellStep { Damage = 8, Color = Color.Green },
                        new ColorSpellStep { Damage = 0, Color = Color.Green },
                    ]
                }
            },
            {
                new ColorSpellType { Color = Color.Green, Length = Length.Long },
                new ColorSpell
                {
                    Length = 4, Steps = [
                        new ColorSpellStep { Damage = 6, Color = Color.Green },
                        new ColorSpellStep { Damage = 9, Color = Color.Green },
                        new ColorSpellStep { Damage = 3, Color = Color.Green },
                        new ColorSpellStep { Damage = 0, Color = Color.Green },
                    ]
                }
            },
            {
                new ColorSpellType { Color = Color.Blue, Length = Length.Short },
                new ColorSpell
                {
                    Length = 2, Steps = [
                        new ColorSpellStep { Damage = 1, Color = Color.Blue },
                        new ColorSpellStep { Damage = 5, Color = Color.Blue },
                    ]
                }
            },
            {
                new ColorSpellType { Color = Color.Blue, Length = Length.Long },
                new ColorSpell
                {
                    Length = 6, Steps = [
                        new ColorSpellStep { Damage = 4, Color = Color.Blue },
                        new ColorSpellStep { Damage = 3, Color = Color.Blue },
                        new ColorSpellStep { Damage = 7, Color = Color.Blue },
                        new ColorSpellStep { Damage = 4, Color = Color.Blue },
                        new ColorSpellStep { Damage = 12, Color = Color.Blue },
                        new ColorSpellStep { Damage = 0, Color = Color.Blue },
                    ]
                }
            },
        };
}