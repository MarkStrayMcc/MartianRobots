using Martian.Robots.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Martian.Robots.Core
{
    public class Robot
    {
        private const char Left = 'L';
        private const char Right = 'R';
        private const char Forward = 'F';
        private const int MaxInstructionLength = 100;

        private readonly IWorld _world;
        public Position CurrentPosition { get; private set; }
        private bool _isLost;

        public Robot(int xCoordinates, int yCoordinates, Orientation orientation, IWorld world)
        {
            if (world == null)
                throw new ArgumentNullException(nameof(world));

            if (xCoordinates < 0 || yCoordinates < 0)
                throw new ArgumentException("Coordinates cannot be negative");

            if (!world.IsPositionValid(xCoordinates, yCoordinates))
                throw new ArgumentException($"Initial position ({xCoordinates}, {yCoordinates}) is out of world boundaries.");

            CurrentPosition = new Position(xCoordinates, yCoordinates, orientation);
            _world = world;
        }

        public void ProcessInstructions(string instructions)
        {
            if (instructions.Length >= MaxInstructionLength)
                throw new ArgumentException($"Instruction string cannot exceed {MaxInstructionLength} characters");

            foreach (var command in instructions)
            {
                if (!IsValidCommand(command))
                    throw new InvalidOperationException($"Invalid instruction '{command}' received.");

                if (_isLost) break;

                ExecuteCommand(command);
            }
        }

        private bool IsValidCommand(char command) =>
            command == Left || command == Right || command == Forward;

        private void ExecuteCommand(char command)
        {
            switch (command)
            {
                case Left:
                    TurnLeft();
                    break;
                case Right:
                    TurnRight();
                    break;
                case Forward:
                    MoveForward();
                    break;
            }
        }

        private void TurnLeft() => CurrentPosition = CurrentPosition with
        {
            Orientation = CurrentPosition.Orientation switch
            {
                Orientation.N => Orientation.W,
                Orientation.W => Orientation.S,
                Orientation.S => Orientation.E,
                Orientation.E => Orientation.N,
                _ => CurrentPosition.Orientation
            }
        };

        private void TurnRight() => CurrentPosition = CurrentPosition with
        {
            Orientation = CurrentPosition.Orientation switch
            {
                Orientation.N => Orientation.E,
                Orientation.E => Orientation.S,
                Orientation.S => Orientation.W,
                Orientation.W => Orientation.N,
                _ => CurrentPosition.Orientation
            }
        };

        private void MoveForward()
        {
            var (x, y, orientation) = CurrentPosition;
            var newPosition = orientation switch
            {
                Orientation.N => (x, y + 1, orientation),
                Orientation.S => (x, y - 1, orientation),
                Orientation.E => (x + 1, y, orientation),
                Orientation.W => (x - 1, y, orientation),
                _ => (x, y, orientation)
            };

            if (!_world.IsPositionValid(newPosition.Item1, newPosition.Item2))
            {
                if (_world.TryRecordMarker(x, y, orientation))
                    _isLost = true;
                return;
            }

            CurrentPosition = new Position(newPosition.Item1, newPosition.Item2, newPosition.orientation);
        }

        public override string ToString() =>
            _isLost ? $"{CurrentPosition} LOST" : CurrentPosition.ToString();
    }
}
