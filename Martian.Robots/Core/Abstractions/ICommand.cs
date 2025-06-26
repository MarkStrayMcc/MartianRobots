using Martian.Robots.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Martian.Robots.Core.Abstractions
{
    public interface ICommand
    {
        Position Execute(Position current, IWorld world);
        char Symbol { get; }
    }
}
