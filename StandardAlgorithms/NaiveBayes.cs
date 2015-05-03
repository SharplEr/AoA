using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VectorSpace;
using IOData;
using AoA;

namespace StandardAlgorithms
{
    /*
    public class NaiveBayes : Algorithm
    {
        Vector[][] pArg;
        Vector pClass;
        double threshold;

        public NaiveBayes()
        {
            
        }

        public override void Learn(SigmentData data)
        {
            Results res = data.GetResults();
            int[] arr = res.Counts;

            pClass = new Vector(arr.Length, (i) => arr[i] / res.Length);

            int[][] input = data.GetDiscreteArray();

            pArg = new Vector[pClass.Length][];

            int[] max = data.GetMaxDiscretePath();

            for (int i = 0; i < pClass.Length; i++)
            {
                pArg[i] = new Vector[data.Dimension];
                for (int j = 0; j < data.Dimension; j++)
                {
                    pArg[i][j] = new Vector(max[j]);
                }
            }

            for (int i = 0; i < data.Length; i++)
            {
                //data.GetMaxDiscretePath
            }
        }

        protected Result Calc(int[] x)
        {
            
        }

        public override Results Calc(SigmentInputData data)
        {
            int[][] x = data.GetDiscreteArray();
        }

        public override void ChangeThreshold(double th)
        {
            threshold = th;
        }
    }*/
}
