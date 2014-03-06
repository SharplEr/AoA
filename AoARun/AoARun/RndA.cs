using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AoA;
using VectorSpace;

namespace AoARun
{
    public class RndA : Algorithm
    {
        Random r = new Random(1578943);

        public RndA() { }

        double th = 0.0;
        int dem;

        public override Vector[] Calc(Vector[] date)
        {
            Vector[] ans = new Vector[date.Length];

            for (int i = 0; i < ans.Length; i++)
                ans[i] = new Vector(dem).SetRandom(-1 + th, 1 + th);

            return ans;
        }

        public override void Learn(Vector[] inputDate, Vector[] resultDate)
        {
            dem = resultDate[0].Length;
        }

        public override double GetThreshold()
        {
            return th;
        }

        public override void ChangeThreshold(double t)
        {
            th = t;
        }
    }
}
