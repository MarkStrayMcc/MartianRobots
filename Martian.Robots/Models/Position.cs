namespace Martian.Robots.Models
{
    public record Position(int X, int Y, Orientation Orientation)
    {
        public bool IsLost { get; init; } = false;
        public override string ToString() => IsLost ? $"{X} {Y} {Orientation} LOST" : $"{X} {Y} {Orientation}";
    }
}
    