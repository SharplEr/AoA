using System;
using AoA;
using IOData;
using VectorSpace;

namespace StandardAlgorithms
{
    public class Neighbour: Algorithm
    {
        Vector[][] obj;

        double threshold = 0;

        public Neighbour(params object[] p) : base(p) { }

        public override Results Calc(SigmentInputData data)
        {
            if (data == null) throw new ArgumentException("data is null");

            Vector[] x = data.GetСontinuousArray();
            return new Results((i) => Calc(x[i]), x.Length);
        }

        public Result Calc(Vector x)
        {
            Vector ans = new Vector(obj.Length);

            for (int i = 0; i < obj.Length; i++)
            {
                int minIndex = 0;
                double min = (double)(x - obj[i][0]);
                for (int j = 1; j < obj[i].Length; j++)
                {
                    double t = (double)(x - obj[i][j]);
                    if (t<min)
                    {
                        min = t;
                        minIndex = j;
                    }
                }
                ans[i] = Math.Sqrt(min);
            }

            ans.InvPer().AddThreshold(threshold);

            return new Result(ans);
        }

        public override void Learn(SigmentData data)
        {
            if (data == null) throw new ArgumentException("data is null");
            Vector[] x = data.GetСontinuousArray();
            Results r = data.GetResults();

            obj = new Vector[r.Counts.Length][];

            int[] indexs = new int[r.Counts.Length];

            for(int i = 0; i < obj.Length; i++)
                obj[i] = new Vector[r.Counts[i]];

            for (int i = 0; i < x.Length; i++)
            {
                int c = r[i].Number;
                obj[c][indexs[c]] = x[i];
                indexs[c]++;
            }
        }

        public override void ChangeThreshold(double th)
        {
            threshold = th;
        }
    }
}
