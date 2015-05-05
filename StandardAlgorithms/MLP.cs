using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VectorSpace;
using IOData;
using Araneam;
using AoA;

namespace StandardAlgorithms
{
    public class MLP: Algorithm, IDisposable
    {
        ClassicNetwork network;
        double r, tm;
        int max;

        int[] ls;

        double threshold = 0;

        public MLP(params object[] p)
        {
            if (p.Length <= 4) throw new ArgumentException("Длина не та");

            int[] a = new int[p.Length - 3];

            for (int i = 3; i < p.Length; i++)
                a[i - 3] = (int)p[i];

            Set((double)p[0], (double)p[1], (int)p[2], a);
        }

        public MLP(double rr, double tt, int mmax, params int[] l)
        {
            Set(rr, tt, mmax, l);
        }

        protected void Set(double rr, double tt, int mmax, params int[] l)
        {
            r = rr;
            tm = tt;
            max = mmax;
            ls = l;
        }

        public override Results Calc(SigmentInputData data)
        {
            if (network == null) throw new NullReferenceException("Сперва должно пройти обучение");

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
            network = new ClassicNetwork(r, tm, ls, inputDate[0].Length, resultDate[0].Length);
            network.AddTestDate(pvsi.ToArray(), pvso.ToArray(), counts);
            network.NewLearn2(false, max);
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
