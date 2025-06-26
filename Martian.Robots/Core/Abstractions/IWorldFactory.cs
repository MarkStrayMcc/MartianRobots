using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Martian.Robots.Core.Abstractions
{
    public interface IWorldFactory
    {
        IWorld Create(int maxX, int maxY);
    }
}
