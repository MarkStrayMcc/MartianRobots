using Martian.Robots.Core.Abstractions;
using Martian.Robots.Models;

namespace Martian.Robots.Core.Commands
{
    public class TurnRightCommand : ICommand
    {
        public char Symbol => 'R';

        public Position Execute(Position current, IWorld world)
        {
            return current with
            {
                Orientation = current.Orientation switch
                {
                    Orientation.N => Orientation.E,
                    Orientation.E => Orientation.S,
                    Orientation.S => Orientation.W,
                    Orientation.W => Orientation.N,
                    _ => current.Orientation
                }
            };
        }
    }
}
