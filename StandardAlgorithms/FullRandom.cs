using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IOData;
using AoA;
using VectorSpace;

namespace StandardAlgorithms
{
    public class FullRandom : Algorithm
    {
        Random r;

        double th = 0;
        int m = 0;

        public FullRandom(object[] o)
        {}

        public override void Learn(SigmentData data)
        {
            m = data.GetResults().MaxNumber;
        }

        public override Results Calc(SigmentInputData date)
        {
            return new Results((i) =>
                {
                    double l = r.NextDouble();
                    l += th;
                    if (l < 0.5) return new Result(0, m);
                    else return new Result(1, m);
                }, date.Length);
        }

        public override void ChangeThreshold(double th)
        {
            this.th = th;
        }
    }
}