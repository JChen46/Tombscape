[SpacetimeDB.Type]
public partial struct DbVector2
{
    public int x;
    public int y;

    public DbVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public float SqrMagnitude => x * x + y * y;
    public float Magnitude => MathF.Sqrt(SqrMagnitude);
    public DbVector2 Normalized => this / Magnitude;

    public static DbVector2 operator +(DbVector2 a, DbVector2 b) => new DbVector2(a.x + b.x, a.y + b.y);
    public static DbVector2 operator -(DbVector2 a, DbVector2 b) => new DbVector2(a.x - b.x, a.y - b.y);
    public static DbVector2 operator *(DbVector2 a, int b) => new DbVector2(a.x * b, a.y * b);
    // modified to round to nearest int
    public static DbVector2 operator /(DbVector2 a, float b) => new DbVector2((int)Math.Round((float)a.x / b), (int)Math.Round(a.y / b));
    
    public static bool operator !=(DbVector2 a, DbVector2 b) => a.x != b.x || a.y != b.y;
    
    public static bool operator ==(DbVector2 a, DbVector2 b) => a.x == b.x && a.y == b.y;
}