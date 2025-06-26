using Martian.Robots.Core.Abstractions;
using Martian.Robots.Core.Services;
using Martian.Robots.Core;
using Martian.Robots.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Martian.Robots
{
    public class Application
    {
        private IWorld _world = null;
        private readonly IWorldFactory _worldFactory;
        private readonly ICommandRegistry _commandRegistry;

        public Application(IWorldFactory worldFactory, ICommandRegistry commandRegistry)
        {
            _worldFactory = worldFactory;
            _commandRegistry = commandRegistry;
        }

        public void Run()
        {
            InitializeWorld();
            ProcessRobots();
        }

        private void InitializeWorld()
        {
            Console.WriteLine("Enter grid bounds (e.g., '5 3'):");
            var boundsInput = Console.ReadLine()?.Split(' ');

            _world = _worldFactory.Create(int.Parse(boundsInput[0]), int.Parse(boundsInput[1]));
        }

        private void ProcessRobots()
        {
            while (true)
            {
                Console.WriteLine("\nEnter robot position (x y orientation) or empty to exit:");
                var positionInput = Console.ReadLine();
                if (string.IsNullOrEmpty(positionInput)) break;

                var positionParts = positionInput.Split(' ');
                var robot = new Robot(
                    xCoordinates: int.Parse(positionParts[0]),
                    yCoordinates: int.Parse(positionParts[1]),
                    orientation: Enum.Parse<Orientation>(positionParts[2]),
                    world: _world,
                    commands: _commandRegistry
                );

                Console.WriteLine("Enter robot instructions:");
                var instructions = Console.ReadLine();
                robot.ProcessInstructions(instructions);

                Console.WriteLine($"Result: {robot}");
            }
        }
    }
}
