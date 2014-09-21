using System;
using VectorSpace;
using Araneam;
using MyParallel;
using IOData;

namespace GenomeNeuralNetwork
{
    public class GenomeNetwork: BackPropagationNetwork
    {
        public readonly static string[] TestTags = new string[] {"Возраст", 
            "Ожирение", "Курение", "Алкоголь", 
            "FV", "FII", "AGTR", "AGT_174", "AGT_235", "PAI1", "MTHFR", "ACE", "NOS", "APOE", 
            "LPL+73in6", "LPL+82in6", "LPL_HindIII", "LPL_S447X", "LIPC(-514)", "LIPCV155V", 
            "CETP_taq", "CETP_I405V"};
        public readonly static string[] FenTags = new string[] {"Возраст"};
        public readonly static string[] GenTags = new string[] {"Ожирение", "Курение", "Алкоголь", "FV", "FII", "AGTR", "AGT_174", "AGT_235", "PAI1", "MTHFR", "ACE", "NOS", "APOE", 
            "LPL+73in6", "LPL+82in6", "LPL_HindIII", "LPL_S447X", "LIPC(-514)", "LIPCV155V", 
            "CETP_taq", "CETP_I405V"};
        public readonly static string[] ResultTags = new string[]{
            "Диагноз"
        };

        double a = 1.7159, b = 2.0 / 3.0;

        Action<Vector> convert;

        public Action<Vector> Convert
        {
            get
            {
                return convert;
            }
        }

        //9-2
        public GenomeNetwork(double r, double t, int one, int two) : base(r, t, 3)
        {
            /*
            layers[0] = new NeuronLayer(one, TestTags.Length + 1, true, 1, "tanh", a, b);
            layers[0].NormalInitialize();
            layers[1] = new NeuronLayer(2, one + 1, false, 1, "tanh", a, b);
            layers[1].NormalInitialize();

            layers[1].CalcInvers(layers[0].WithThreshold);*/
            
            layers[0] = new NeuronLayer(one, TestTags.Length + 1, true, 1, "tanh", a/2, b);
            layers[0].NormalInitialize(random);
            layers[1] = new NeuronLayer(two, one + 1, true, 1, "tanh", a, b);
            layers[1].NormalInitialize(random);
            layers[2] = new NeuronLayer(2, two + 1, false, 1, "tanh", a, b);
            layers[2].NormalInitialize(random);

            layers[1].CalcInvers(layers[0].WithThreshold);
            layers[2].CalcInvers(layers[1].WithThreshold);
        }

        public override LearnLog EarlyStoppingLearn(bool flag)
        {
            return base.EarlyStoppingLearn(flag);
        }

        public override LearnLog NewLearn(bool flag, int max)
        {
            return base.NewLearn(flag, max);
        }

        public override LearnLog FullLearn()
        {
            return base.FullLearn();
        }

        public override LearnLog FullLearn(double minError)
        {
            return base.FullLearn(minError);
        }
        
        public static Vector[] LoadResultDate(CSVReader reader)
        {
            Vector[] ans = new Vector[reader.countLine];
            for (int i = 0; i < ans.Length; i++)
                ans[i] = new Vector(ResultTags.Length, (j) => { return ToDouble(reader[i, ResultTags[j]]); });
            return ans;
        }

        public static double ToDouble(string s)
        {
            double ans;

            if ((s[0] == '-') && (s.Length == 1))
            {
                ans = 0;
                return ans;
            }

            if (s[0] == '<')
            {
                string t = "";
                for (int i = 1; i < s.Length; i++)
                    t += s[i];
                s = t;
            }

            if (!Double.TryParse(s, out ans))
            {
                if (s == "отрицат") ans = -1.0;
                else ans = 1.0;
            }

            return ans;
        }

        public NeuronLayer[] getLayers()
        {
            return layers;
        }
    }
}