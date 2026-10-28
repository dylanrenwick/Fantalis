namespace Fantalis.Core.Math;

public struct Vector3
{
    public static readonly Vector3 Zero = new(0, 0, 0);

    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public Vector3(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static Vector3 operator +(Vector3 a, Vector3 b)
    {
        return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }
    public static Vector3 operator -(Vector3 a, Vector3 b)
    {
        return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }
    public static Vector3 operator *(Vector3 a, double b)
    {
        return new Vector3(a.X * b, a.Y * b, a.Z * b);
    }
    public static Vector3 operator *(Vector3 a, Vector3 b)
    {
        return new Vector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
    }
    public static Vector3 operator /(Vector3 a, double b)
    {
        return new Vector3(a.X / b, a.Y / b, a.Z / b);
    }
    public static Vector3 operator /(Vector3 a, Vector3 b)
    {
        return new Vector3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
    }

    public static implicit operator Vector3(Vector2 a)
    {
        return new Vector3(a.X, a.Y, 0);
    }
}
