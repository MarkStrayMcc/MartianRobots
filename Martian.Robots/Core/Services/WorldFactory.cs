using Martian.Robots.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Martian.Robots.Core.Services
{
    public class WorldFactory : IWorldFactory
    {
        public IWorld Create(int maxX, int maxY) => new World(maxX, maxY);
    }
}
