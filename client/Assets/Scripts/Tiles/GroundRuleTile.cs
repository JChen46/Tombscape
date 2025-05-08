using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class GroundRuleTile : RuleTile<GroundRuleTile.Neighbor>
{
    public readonly bool isWalkable = true; // these custom properties apply to all GroundRuleTiles

    public class Neighbor : RuleTile.TilingRule.Neighbor {
        public const int Null = 3;
        public const int NotNull = 4;
    }

    public override bool RuleMatch(int neighbor, TileBase tile) {
        switch (neighbor) {
            case Neighbor.Null: return tile == null;
            case Neighbor.NotNull: return tile != null;
        }
        return base.RuleMatch(neighbor, tile);
    }
}