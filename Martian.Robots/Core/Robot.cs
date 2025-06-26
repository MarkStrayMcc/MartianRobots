using Martian.Robots.Core.Abstractions;
using Martian.Robots.Core.Services;
using Martian.Robots.Models;

namespace Martian.Robots.Core
{
    public class Robot
    {
        public const int MaxInstructionLength = 100;
        private readonly ICommandRegistry _commands;

        private readonly IWorld _world;
        public Position CurrentPosition { get; private set; }
        private bool _isLost;

        public Robot(int xCoordinates, int yCoordinates, Orientation orientation, IWorld world, ICommandRegistry commands)
        {
            if (world == null)
                throw new ArgumentNullException(nameof(world));

            if (xCoordinates < 0 || yCoordinates < 0)
                throw new ArgumentException("Coordinates cannot be negative");

            if (!world.IsPositionValid(xCoordinates, yCoordinates))
                throw new ArgumentException($"Initial position ({xCoordinates}, {yCoordinates}) is out of world boundaries.");

            CurrentPosition = new Position(xCoordinates, yCoordinates, orientation);

            _commands = commands;
            _world = world;
        }

        public void ProcessInstructions(string instructions)
        {
            if (instructions.Length >= MaxInstructionLength)
                throw new ArgumentException($"Instruction string cannot exceed {MaxInstructionLength} characters");

            foreach (var symbol in instructions)
            {
                if (CurrentPosition.IsLost) break;

                var command = _commands.GetCommand(symbol);
                CurrentPosition = command.Execute(CurrentPosition, _world);
            }
        }

        public override string ToString() => CurrentPosition.ToString();
    }
}
