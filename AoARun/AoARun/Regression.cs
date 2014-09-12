using System;
using VectorSpace;
using AoA;
using Araneam;
using GenomeNeuralNetwork;
using IOData;

namespace AoARun
{
    /*
    public class Regression: Algorithm
    {
        reg network;
        double r, tm;
        public Regression(double rr, double tt)
        {
            r = rr;
            tm = tt;
            name = "Линейная регрессия";
        }

        public override Vector Calc(Vector date)
        {
            if (network == null) throw new NullReferenceException("Сперва должно пройти обучение");

            return network.Calculation(date);
        }

        public override void Learn(SigmentData data)
        {
            Vector[] inputDate = data.GetСontinuousArray();
            Vector[] resultDate = data.GetResults().ToSpectrums();

            if (network != null) network.Dispose();

            int count=0;

            for (int i = 0; i < resultDate.Length; i++)
            {
                if (resultDate[i][0] == 1) count++;
            }
            /*
            Vector tv1;
            Vector tv2;
            int first = 0, end = count;
            while(end<resultDate.Length)
            {
                if (resultDate[first][0] == -1)
                {
                    tv1 = resultDate[first];
                    resultDate[first] = resultDate[end];
                    resultDate[end] = tv1;

                    tv2 = inputDate[first];
                    inputDate[first] = inputDate[end];
                    inputDate[end] = tv2;

                    end++;
                }
                else first++;
            }

            int [] counts = new int[]{count, resultDate.Length-count};
            */
    /*
            network = new reg(r, tm, 1, GenomeNetwork.TestTags.Length + 1, "no");
            for(int j = 0; j<500; j++)
            for(int i = 0; i<inputDate.Length; i++)
                network.Learn(new Vector(inputDate[i].Length, (m)=>inputDate[i][m], 0.5), resultDate[i]);
        }

        public override void ChangeThreshold(double th)
        {
            NeuronLayer[] nls = network.getLayers();
            Neuron n = nls[nls.Length - 1].neuros[nls[nls.Length - 1].neuros.Length - 1];
            n.weight[n.Length - 1] = th / n.synapse[n.Length - 1];
        }

        public override double GetThreshold()
        {
            NeuronLayer[] nls = network.getLayers();
            Neuron n = nls[nls.Length - 1].neuros[nls[nls.Length - 1].neuros.Length - 1];
            return n.weight[n.Length - 1] * n.synapse[n.Length - 1];
        }

        public override void Dispose()
        {
            network.Dispose();
        }


        class reg : LMSNetwork
        {
            public reg(double r, double t, int n, int m, string name, params Double[] p) : base(r, t, n, m, name, p) { }
            public NeuronLayer[] getLayers()
            {
                return RLayers;
            }
        }
    }*/
}
