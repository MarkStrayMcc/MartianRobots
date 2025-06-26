namespace Martian.Robots.Models
{
    public record Position(int X, int Y, Orientation Orientation)
    {
        public override string ToString() => $"{X} {Y} {Orientation}";
    }
}
