using SpacetimeDB.Types;
using UnityEngine;

namespace SpacetimeDB.Types
{
	public partial class DbVector2
	{
		public static implicit operator Vector2(DbVector2 vec)
		{
			return new Vector2(vec.X, vec.Y);
		}

		public static implicit operator DbVector2(Vector2 vec)
		{
			// added explicit cast
			return new DbVector2((int)vec.x, (int)vec.y);
		}
	}
}