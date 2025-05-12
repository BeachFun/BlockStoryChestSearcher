struct Vector3
{
    public int X;
    public int Y;
    public int Z;

    public Vector3(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static double Distance(Vector3 a, Vector3 b)
    {
        int dx = a.X - b.X;
        int dy = a.Y - b.Y;
        int dz = a.Z - b.Z;
        return Math.Sqrt(dx * dx + dy * dy + dz * dz);
    }

    public override string ToString()
    {
        return $"{X}/{Y}/{Z}";
    }
}