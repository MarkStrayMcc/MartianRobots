using Martian.Robots.Core.Abstractions;
using Martian.Robots.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Martian.Robots.Core.Commands
{
    public class TurnLeftCommand : ICommand
    {
        public char Symbol => 'L';

        public Position Execute(Position current, IWorld world)
        {
            return current with
            {
                Orientation = current.Orientation switch
                {
                    Orientation.N => Orientation.W,
                    Orientation.W => Orientation.S,
                    Orientation.S => Orientation.E,
                    Orientation.E => Orientation.N,
                    _ => current.Orientation
                }
            };
        }
    }
}
