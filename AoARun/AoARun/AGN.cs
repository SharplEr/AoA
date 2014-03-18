using System;
using System.Collections.Generic;
using VectorSpace;
using AoA;
using Araneam;
using GenomeNeuralNetwork;

namespace AoARun
{
    class AGN: Algorithm
    {
        static int globalcount = 0;
        object naming = new object();
        GenomeNetwork network;
        double r, tm;
        int max;

        public AGN(double rr, double tt, int mmax)
        {
            r = rr;
            tm = tt;
            max = mmax;
            lock (naming)
            {
                globalcount++;
                name = "Многослойный персептрон №" + globalcount.ToString();
            }
        }

        public override Vector[] Calc(Vector[] date)
        {
            if (network == null) throw new NullReferenceException("Сперва должно пройти обучение");

            return network.Calculation(date);
        }

        public override void Learn(Vector[] inputDate, Vector[] resultDate)
        {
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
            //resultDate = pvso.ToArray();
            //inputDate = pvsi.ToArray();

            int[] counts = new int[] { count, resultDate.Length - count };
            network = new GenomeNetwork(r, tm);
            network.AddTestDate(pvsi.ToArray(), pvso.ToArray(), counts);
            network.NewLearn(false, max);
            //network.EarlyStoppingLearn(false);
        }

        public override void ChangeThreshold(double th)
        {
            NeuronLayer[] nls = network.getLayers();
            Neuron n = nls[nls.Length - 1].neuros[nls[nls.Length - 1].neuros.Length - 1];
            n.weight[n.Length - 1]  = th / n.synapse[n.Length - 1];
        }

        public override double GetThreshold()
        {
            NeuronLayer[] nls = network.getLayers();
            Neuron n = nls[nls.Length - 1].neuros[nls[nls.Length - 1].neuros.Length - 1];
            return n.weight[n.Length-1]*n.synapse[n.Length-1];
        }

        public override void Dispose()
        {
            if (network!=null)
                network.Dispose();
        }
    }
}
