using System;
using VectorSpace;
using Araneam;
using MyParallel;
using IODate;

namespace GenomeNeuralNetwork
{
    public class GenomeNetwork: BackPropagationNetwork
    {
        public readonly static string[] TestTags = new string[] {"Возраст", 
            "Ожирение", "Курение", "Алкоголь", 
            "FV", "FII", "AGTR", "AGT_174", "AGT_235", "PAI1", "MTHFR", "ACE", "NOS", "APOE", 
            "LPL+73in6", "LPL+82in6", "LPL_HindIII", "LPL_S447X", "LIPC(-514)", "LIPCV155V", 
            "CETP_taq", "CETP_I405V"};
        public readonly static string[] FenTags = new string[] {"Возраст", 
            "Ожирение", "Курение", "Алкоголь"};
        public readonly static string[] GenTags = new string[] {"FV", "FII", "AGTR", "AGT_174", "AGT_235", "PAI1", "MTHFR", "ACE", "NOS", "APOE", 
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

        DateReader Reader;
        //9-2
        public GenomeNetwork(double r, double t) : base(r, t, 3)
        {
            layers[0] = new NeuronLayer(9, TestTags.Length + 1, true, 1, "tanh", a, b);
            layers[0].NormalInitialize();
            layers[1] = new NeuronLayer(2, 9 + 1, true, 1, "tanh", a, b);
            layers[1].NormalInitialize();
            layers[2] = new NeuronLayer(ResultTags.Length, 2 + 1, false, 1, "tanh", a, b);
            layers[2].NormalInitialize();

            layers[1].CalcInvers(layers[0].WithThreshold);
            layers[2].CalcInvers(layers[1].WithThreshold);
        }

        public bool Reload(string[] names)
        {
            if (names == null) return false;
            if (names.Length == 0) return true;
            try
            {
                DateAnalysis analysis = new DateAnalysis(names, TestTags, ResultTags, FenTags, (s) =>
                {
                    if (s[0] == "отрицат") return -1.0;
                    else return 1.0;
                }, ToDouble);

                resultDate = analysis.ResultDate;
                inputDate = analysis.TestDate;
                convert = analysis.Convert;
                Reader = analysis.Reader;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override LearnLog EarlyStoppingLearn(bool flag)
        {
            return base.EarlyStoppingLearn(flag);
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

        public Vector[] Calculation(String name)
        {
            Reader.Read(name);
            Vector[] answer = new Vector[Reader.TestDate.Length];
            
            for (int i = 0; i < answer.Length; i++)
            {

                answer[i] = Calculation(Reader.TestDate[i]).CloneOk();
            }

            return answer;
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