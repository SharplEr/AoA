using System;
using System.Collections.Generic;
using AoA;
using Araneam;
using IOData;
using VectorSpace;

namespace StandardAlgorithms
{
    public class TwoLayerNetwork : Algorithm, IDisposable
    {
        protected readonly ProblemMod mod = ProblemMod.classification;
        ClassicNetwork network;
        double r, tm;
        int max;

        int l;

        double threshold = 0;

        public TwoLayerNetwork(params object[] p)
            : base(ProblemMod.classification)
        {
            if (p == null) throw new ArgumentException("p is null");
            if (p.Length != 4) throw new ArgumentException("Длина не та");
            Set((double)p[0], (double)p[1], (int)p[2], (int)p[3]);
        }

        public TwoLayerNetwork(double rr, double tt, int mmax, int one)
            : base(ProblemMod.classification)
        {
            Set(rr, tt, mmax, one);
        }

        protected void Set(double rr, double tt, int mmax, int one)
        {
            r = rr;
            tm = tt;
            max = mmax;
            l = one;
        }

        protected override Results Classification(SigmentInputData data)
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

            if (network != null) network.Dispose();

            List<Vector> pvso = new List<Vector>();
            List<Vector> nvso = new List<Vector>();
            List<Vector> pvsi = new List<Vector>();
            List<Vector> nvsi = new List<Vector>();

            for (int i = 0; i < resultDate.Length; i++)
            {
                if (resultDate[i][0] == 1.0)
                {
                    pvso.Add(resultDate[i]);
                    pvsi.Add(inputDate[i]);
                }
                else
                {
                    nvso.Add(resultDate[i]);
                    nvsi.Add(inputDate[i]);
                }
            }
            int count = pvso.Count;
            pvso.AddRange(nvso);
            pvsi.AddRange(nvsi);
 
            int[] counts = new int[] { count, resultDate.Length - count };
            network = new ClassicNetwork(r, tm, new int[] { l }, inputDate[0].Length, resultDate[0].Length);
            network.AddTestDate(pvsi.ToArray(), pvso.ToArray(), counts);
            network.NewLearn(false, max);
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
