using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorSpace;
using Araneam;
using MyParallel;
using IODate;

namespace GenomeNeuralNetwork
{
    public class GenomeNetwork: BackPropagationNetwork
    {
        readonly static string[] TestTags = new string[] {"Возраст", 
            "Ожирение", "Курение", "Алкоголь", 
            "FV", "FII", "AGTR", "AGT_174", "AGT_235", "PAI1", "MTHFR", "ACE", "NOS", "APOE", 
            "LPL+73in6", "LPL+82in6", "LPL_HindIII", "LPL_S447X", "LIPC(-514)", "LIPCV155V", 
            "CETP_taq", "CETP_I405V"};
        readonly static string[] FenTags = new string[] {"Возраст", 
            "Ожирение", "Курение", "Алкоголь"};
        readonly static string[] GenTags = new string[] {"FV", "FII", "AGTR", "AGT_174", "AGT_235", "PAI1", "MTHFR", "ACE", "NOS", "APOE", 
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

        DateInfo info;

        public GenomeNetwork(double r, double t) : base(r, t, 3)
        {           
            hidden[0] = new NeuronLayer(9, TestTags.Length+1, true, "tanh", a, b);
            hidden[0].NormalInitialize();            
            hidden[1] = new NeuronLayer(2, 9+1, true,"tanh", a, b);
            hidden[1].NormalInitialize();
            hidden[2] = new NeuronLayer(ResultTags.Length, 2 + 1, false, "tanh", a, b);
            hidden[2].NormalInitialize();
             
            hidden[1].CalcInvers(hidden[0].WithThreshold);
            hidden[2].CalcInvers(hidden[1].WithThreshold);
        }

        public bool Reload(string[] names)
        {
            if (names == null) return false;
            if (names.Length == 0) return true;
            try
            {
                CSVReader reader;

                List<Vector> nTests = new List<Vector>();
                //List<Vector> nResult = new List<Vector>();
                List<string> t = ResultTags.ToList();
                t.AddRange(TestTags);
                reader = new CSVReader(t.ToArray(), names);
                if (!reader.Test()) return false;

                info = new DateInfo(reader, GenTags, ResultTags, (s) =>
                {
                    if (s[0] == "отрицат") return -1.0;
                    else return 1.0;
                });

                
                for (int i = 0; i < reader.countLine; i++)
                {
                    nTests.Add(new Vector(TestTags.Length, (j) =>
                    {
                        if (j<FenTags.Length)
                            return ToDouble(reader[i, TestTags[j]]);
                        else
                            return info[TestTags[j], reader[i, TestTags[j]]];
                    }, 1.0));
                }
                

             //   testDate = LoadTestDate(reader);
                testDate = nTests.ToArray();
                resultDate = LoadResultDate(reader);

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

        /*
        public static Vector[] LoadTestDate(CSVReader reader)
        {
            Vector[] ans = new Vector[reader.countLine];
            for (int i = 0; i < ans.Length; i++)
            {
                ans[i] = new Vector(TestTags.Length, (j) => { return ToDouble(reader[i, TestTags[j]]); }, 1.0);
            }
            return ans;
        }*/

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
            Vector v;
            Vector t = null;
            for (int i = 0; i < answer.Length; i++)
            {
                v = new Vector(TestTags.Length, (j) =>
                {
                    if (j < FenTags.Length)
                        return ToDouble(reader[i, TestTags[j]]);
                    else
                        return info[TestTags[j], reader[i, TestTags[j]]];
                }, 1.0);

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
