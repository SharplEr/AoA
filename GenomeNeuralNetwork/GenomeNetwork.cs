using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorSpace;
using Araneam;
using MyParallel;

namespace GenomeNeuralNetwork
{
    public class GenomeNetwork: BackPropagationNetwork
    {
        readonly static string[] TestTags = new string[] {"Возраст", 
            "Ожирение", "Курение", "Алкоголь", 
            "FV", "FII", "AGTR", "AGT_174", "AGT_235", "PAI1", "MTHFR", "ACE", "NOS", "APOE", 
            "LPL+73in6", "LPL+82in6", "LPL_HindIII", "LPL_S447X", "LIPC(-514)", "LIPCV155V", 
            "CETP_taq", "CETP_I405V"};
        readonly static string[] ResultTags = new string[]{
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

        public GenomeNetwork(double r, double t) : base(r, t, 3)
        {
            //TestTags.Length=22
            
            
            hidden[0] = new NeuronLayer(9, TestTags.Length+1, true, "tanh", a, b);
            hidden[0].NormalInitialize();            
            hidden[1] = new NeuronLayer(2, 9+1, true,"tanh", a, b);
            hidden[1].NormalInitialize();
            hidden[2] = new NeuronLayer(ResultTags.Length, 2 + 1, false, "tanh", a, b);
            hidden[2].NormalInitialize();
             
            hidden[1].CalcInvers(hidden[0].WithThreshold);
            hidden[2].CalcInvers(hidden[1].WithThreshold);
            

            /*
            hidden[0] = new NeuronLayer(50, TestTags.Length + 1, true, "tanh", a/9, b);
            hidden[0].NormalInitialize();
            hidden[1] = new NeuronLayer(40, 50 + 1, true, "tanh", a/9, b);
            hidden[1].NormalInitialize();
            hidden[2] = new NeuronLayer(30, 40 + 1, true, "tanh", a / 4, b);
            hidden[2].NormalInitialize();
            hidden[3] = new NeuronLayer(20, 30 + 1, true, "tanh", a / 4, b);
            hidden[3].NormalInitialize();
            hidden[4] = new NeuronLayer(10, 20 + 1, true, "tanh", a/2 , b);
            hidden[4].NormalInitialize();
            hidden[5] = new NeuronLayer(5, 10 + 1, true, "tanh", a, b);
            hidden[5].NormalInitialize();
            hidden[6] = new NeuronLayer(ResultTags.Length, 5 + 1, false, "tanh", a, b);
            hidden[6].NormalInitialize();

            hidden[1].CalcInvers(hidden[0].WithThreshold);
            hidden[2].CalcInvers(hidden[1].WithThreshold);
            hidden[3].CalcInvers(hidden[2].WithThreshold);
            hidden[4].CalcInvers(hidden[3].WithThreshold);
            hidden[5].CalcInvers(hidden[4].WithThreshold);
            hidden[6].CalcInvers(hidden[5].WithThreshold);
            */
        }

        public bool Reload(string[] names)
        {
            if (names == null) return false;
            if (names.Length == 0) return true;
            try
            {
                CSVReader reader;
                
                List<Vector> nTests = new List<Vector>();
                List<Vector> nResult = new List<Vector>();
                for (int i = 0; i < names.Length; i++)
                {
                    reader = new CSVReader(names[i]);
                    if (!reader.Test()) return false;

                    nTests.AddRange(LoadTestDate(reader));
                    nResult.AddRange(LoadResultDate(reader));
                }
                testDate = nTests.ToArray();
                resultDate = nResult.ToArray();

                testCount = new int[testDate.Length];

                return true;
            }
            catch
            {
                return false;
            }
        }

        public override void EarlyStoppingLearn()
        {
            convert = testDate.Normalization();

            base.EarlyStoppingLearn();
        }

        public override double FullLearn()
        {
            convert = testDate.Normalization();

            return base.FullLearn();
        }

        public override double FullLearn(double minError)
        {
            convert = testDate.Normalization();

            return base.FullLearn(minError);
        }

        public static Vector[] LoadTestDate(CSVReader reader)
        {
            Vector[] ans = new Vector[reader.countLine];
            for (int i = 0; i < ans.Length; i++)
            {
                ans[i] = new Vector(TestTags.Length, (j) => { return ToDouble(reader[i, TestTags[j]]); }, 1.0);
            }
            return ans;
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
            CSVReader reader = new CSVReader(name);
            Vector[] answer = new Vector[reader.countLine];
            //Vector[] answer2 = new Vector[reader.countLine];
            Vector v;
            Vector t = null;
            for (int i = 0; i < answer.Length; i++)
            {
                v = new Vector(TestTags.Length, (j) => { return ToDouble(reader[i, TestTags[j]]); }, 0.5);

                t = v;
                convert(v);

                answer[i] = (Vector)Calculation(v).Clone();
                
            }

            return answer;
        }

        static double ToDouble(string s)
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
                ans = (double)(ToInt(s[0]) * 7 + ToInt(s[1])) / 48.0;

                if (ans < 1.0/6.0)
                {
                    if (s == "отрицат") ans = -1.0;
                    else ans = 1.0;
                }
            }

            return ans;
        }

        static int ToInt(char c)
        {
            switch (c)
            {
                    /*
                case 'A': return 1;
                case 'C': return 2;
                case 'D': return 3;
                case 'T': return 4;
                case 'G': return 5;
                case 'I': return 6;
                default: return -1;
                     */ 
                case 'A': return 1;
                case 'C': return 2;
                case 'D': return 3;
                case 'I': return 4;
                case 'G': return 5;
                case 'T': return 6;
                default: return -1;
            }
        }
    }
}
