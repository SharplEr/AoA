using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metaheuristics
{
    public interface IJump
    {
        bool Jump(double x, double y);
    }
}
