using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AoA;
using VectorSpace;
using IOData;

namespace StandardAlgorithms
{
    public class RegTest: Algorithm
    {
        public RegTest(params object[] p): base(ProblemMod.regression)
        { }

        public override void Learn(SigmentData data)
        {
        }

        protected override double[] Regression(SigmentInputData data)
        {
            String[][] str = data.GetStringsArray();
            double[] ans = new double[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                ans[i] = Convert.ToDouble(str[i][0]);
            }
            return ans;
        }

        public override void ChangeThreshold(double th)
        {
        }
    }
}
