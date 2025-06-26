using Martian.Robots.Core.Abstractions;
using Martian.Robots.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Martian.Robots.Core
{
    public class World : IWorld
    {
        private const int MaxCoordinateValue = 50;
        private readonly ConcurrentDictionary<string, byte> _scentMarkers = new();

        public int MaxX { get; }
        public int MaxY { get; }

        public World(int maxX, int maxY)
        {
            if (maxX > MaxCoordinateValue || maxY > MaxCoordinateValue)
                throw new ArgumentException($"Coordinates cannot exceed {MaxCoordinateValue}");

            MaxX = maxX;
            MaxY = maxY;
        }

        public bool IsPositionValid(int x, int y) =>
            x >= 0 && y >= 0 && x <= MaxX && y <= MaxY;

        public bool TryRecordMarker(int x, int y, Orientation orientation)
        {
            var scentKey = $"{x},{y},{orientation}";
            return _scentMarkers.TryAdd(scentKey, 0); // Returns true only if this is the first time
        }
    }
}
