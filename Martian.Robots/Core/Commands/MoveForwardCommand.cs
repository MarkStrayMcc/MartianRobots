using Martian.Robots.Core.Abstractions;
using Martian.Robots.Models;

namespace Martian.Robots.Core.Commands
{
    public class MoveForwardCommand : ICommand
    {
        public char Symbol => 'F';

        public Position Execute(Position current, IWorld world)
        {
            var newPosition = CalculateNewPosition(current);

            if (!world.IsPositionValid(newPosition.X, newPosition.Y))
            {
                bool isFirstFall = world.TryRecordMarker(current.X, current.Y, current.Orientation);
                return current with { IsLost = isFirstFall };
            }

            return newPosition;
        }

        private Position CalculateNewPosition(Position current)
        {
            return current.Orientation switch
            {
                Orientation.N => current with { Y = current.Y + 1 },
                Orientation.E => current with { X = current.X + 1 },
                Orientation.S => current with { Y = current.Y - 1 },
                Orientation.W => current with { X = current.X - 1 },
                _ => throw new InvalidOperationException($"Unknown orientation: {current.Orientation}")
            };
        }
    }
}