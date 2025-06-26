using Martian.Robots.Models;

namespace Martian.Robots.Core
{
    public interface IWorld
    {
        int MaxX { get; }
        int MaxY { get; }

        bool IsPositionValid(int x, int y);
        bool TryRecordMarker(int x, int y, Orientation orientation);
    }
}