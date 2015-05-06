using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metaheuristics;

namespace StandardAlgorithms
{
    public class RegSetter:ParametersSeter
    {
        public new static Parameter[] GetParameters()
        {
            const double ir = Math.PI / 7.0 * 22.0;
            const double ir2 = Math.E * 71.0 / 193.0;
            Parameter[] p = new Parameter[3];
            p[0] = new Parameter(500, 1, "начальный коэффициент", (x) => ir * x / 500.0);
            p[1] = new Parameter(32, 1, "время обучения", (x) => ir2 * x * 2.0);
            p[2] = new Parameter(10, 1, "Эпох обучения", (x) => x);
            return p;
        }
    }
}
