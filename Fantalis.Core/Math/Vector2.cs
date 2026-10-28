namespace Fantalis.Core.Math;

public struct Vector2
{
    public static readonly Vector2 Zero = new(0, 0);

    public double X { get; set; }
    public double Y { get; set; }

    public Vector2(double x, double y)
    {
        X = x;
        Y = y;
    }

    public static Vector2 operator +(Vector2 a, Vector2 b)
    {
        return new Vector2(a.X + b.X, a.Y + b.Y);
    }
    public static Vector2 operator -(Vector2 a, Vector2 b)
    {
        return new Vector2(a.X - b.X, a.Y - b.Y);
    }
    public static Vector2 operator *(Vector2 a, double b)
    {
        return new Vector2(a.X * b, a.Y * b);
    }
    public static Vector2 operator *(Vector2 a, Vector2 b)
    {
        return new Vector2(a.X * b.X, a.Y * b.Y);
    }
    public static Vector2 operator /(Vector2 a, double b)
    {
        return new Vector2(a.X / b, a.Y / b);
    }
    public static Vector2 operator /(Vector2 a, Vector2 b)
    {
        return new Vector2(a.X / b.X, a.Y / b.Y);
    }

    public static explicit operator Vector2(Vector3 a)
    {
        return new Vector2(a.X, a.Y);
    }
}
