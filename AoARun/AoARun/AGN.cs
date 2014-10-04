using System;
using System.Collections.Generic;
using VectorSpace;
using AoA;
using Araneam;
using GenomeNeuralNetwork;
using IOData;

namespace AoARun
{
    class AGN: Algorithm
    {
        static int globalcount = 0;
        object naming = new object();
        GenomeNetwork network;
        double r, tm;
        int max;

        int one, two;

        double threshold = 0;

        public AGN(double rr, double tt, int mmax, int one,int two)
        {
            r = rr;
            tm = tt;
            max = mmax;
            this.one = one;
            this.two = two;
            lock (naming)
            {
                globalcount++;
                name = "Многослойный персептрон №" + globalcount.ToString();
            }
        }

        public override Results Calc(SigmentInputData data)
        {
            if (network == null) throw new NullReferenceException("Сперва должно пройти обучение");

            Vector[] ans = network.Calculation(data.GetСontinuousArray());

            Vector m = new Vector(2);
            /*
            m[0] = threshold * 0.5;
            m[1] = -threshold * 0.5;
            */

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
            network = new GenomeNetwork(r, tm, one, two);
            network.AddTestDate(pvsi.ToArray(), pvso.ToArray(), counts);
            network.NewLearn(false, max);
            /*
            int[] lalala = network.UnusedInput(1.0);
            if (lalala == null || lalala.Length == 0) return;
            Console.Write("Не используем:");
            for (int i = 0; i < lalala.Length; i++)
                Console.Write(" {0},", lalala[i]);
            Console.WriteLine();
            */
            
            //if (network.haveNaN()) throw new ArithmeticException("Была ошибка в вычислениях");
        }

        public override void ChangeThreshold(double th)
        {
            //threshold = 1.7159*2.0*th;
            threshold = th;
        }

        public override void Dispose()
        {
            if (network!=null)
                network.Dispose();
        }
    }
}