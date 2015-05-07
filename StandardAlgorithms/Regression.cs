using System;
using AoA;
using Araneam;
using IOData;
using VectorSpace;

namespace StandardAlgorithms
{
    public class Regression: Algorithm, IDisposable
    {
        LMSNetwork network;
        double r, tm;
        int max;
        double threshold = 0;

        public Regression(params object[] p):base(p)
        {
            if (p == null) throw new ArgumentException("p is null");
            if (p.Length != 3) throw new ArgumentException("Длина не та");
            Set((double)p[0], (double)p[1], (int)p[2]);
        }

        public Regression(double rr, double tt, int mmax)
        {
            Set(rr, tt, mmax);
        }

        protected void Set(double rr, double tt, int mmax)
        {
            r = rr;
            tm = tt;
            max = mmax;
        }

        public override Results Calc(SigmentInputData data)
        {
            if (network == null) throw new NullReferenceException("Сперва должно пройти обучение");
            if (data == null) throw new ArgumentException("data is null");

            Vector[] ans = network.Calculation(data.GetСontinuousArray());

            Vector m = new Vector(2);

            for (int i = 0; i < ans.Length; i++)
            {
                m[0] = (Math.Sign(threshold) - ans[i][0]) * Math.Abs(threshold);
                m[1] = (-Math.Sign(threshold) - ans[i][1]) * Math.Abs(threshold);

                ans[i].Addication(m);
            }
            return new Results((i) => new Result(ans[i]), ans.Length);
        }
        
        public override void Learn(SigmentData data)
        {
            if (data == null) throw new ArgumentException("data is null");
            threshold = 0;
            Vector[] inputDate = data.GetСontinuousArray();
            Vector[] resultDate = data.GetResults().ToSpectrums();

            double a = 1.7159, b = 2.0 / 3.0;

            network = new LMSNetwork(r, tm, 2, data.Dimension, "tanh", a, b);
            
            network.Learn(inputDate, resultDate, max);
        }

        public override void ChangeThreshold(double th)
        {
            threshold = th;
        }

        public override void Dispose()
        {
            if (network!=null)
                network.Dispose();
        }
    }
}
