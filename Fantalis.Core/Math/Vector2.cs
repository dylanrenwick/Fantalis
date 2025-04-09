namespace Fantalis.Core.Math;

public struct Vector2
{
    public static readonly Vector2 ZERO = new(0, 0);

    public double X { get; set; }
    public double Y { get; set; }

    public Vector2(double x, double y)
    {
        X = x;
        Y = y;
    }
}
