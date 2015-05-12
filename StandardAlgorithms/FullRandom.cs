using System;
using AoA;
using IOData;

namespace StandardAlgorithms
{
    public class FullRandom : Algorithm
    {
        readonly Random r = new Random(777);

        double threshold = 0;
        int m = 0;

        public FullRandom(object[] o) : base(ProblemMod.classification)
        {
        }

        public override void Learn(SigmentData data)
        {
            if (data == null) throw new ArgumentException("data is null");
            m = data.GetResults().MaxNumber;
        }

        protected override Results Classification(SigmentInputData data)
        {
            if (data == null) throw new ArgumentException("data is null");

            return new Results((i) =>
                {
                    double l = r.NextDouble();
                    l += threshold;
                    if (l < 0.5) return new Result(0, m);
                    else return new Result(1, m);
                }, data.Length);
        }

        public override void ChangeThreshold(double th)
        {
            this.threshold = th;
        }
    }
}