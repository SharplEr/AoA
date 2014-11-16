using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AoA;
using IOData;
using Araneam;
using VectorSpace;

namespace AoARun
{
    public class AGRNN: Algorithm
    {
        RNNetwork network;

        double threshold = 0;

        int one = 24;
        int two = 9;
        int m = 29;
        int s = 11;
        double x = 4.05;

        public AGRNN()
        {}

        public AGRNN(int o, int t, int mm, int s, double xx)
        {
            one = o;
            two = t;
            m = mm;
            this.s = s;
            x = xx;
        }

        public AGRNN(object[] o)
        {
            one = (int) o[0];
            two = (int) o[1];
            m = (int) o[2];
            s = (int) o[3];
            x = (double)o[4];
        }

        public override Results Calc(SigmentInputData data)
        {
            if (network == null) throw new NullReferenceException("Сперва должно пройти обучение");

            Vector[] ans = network.Calculation(data.GetMixArray());
            
            Vector m = new Vector(2);
            
            /*
            m[0] = +threshold * 0.5;
            m[1] = -threshold * 0.5;
            */

            m[0] = (Math.Sign(threshold) - m[0]) * Math.Abs(threshold/2);
            m[1] = (-Math.Sign(threshold) - m[1]) * Math.Abs(threshold/2);

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
            threshold = 0.0;

            network = new RNNetwork();
            network.Learning(data.GetMixArray(), data.GetResults(), data.GetMaxDiscretePath(), s, 0.9, one, two, m, x);
        }

        public override void ChangeThreshold(double th)
        {
            threshold = th;
        }

        public override void Dispose()
        {
            if (network != null)
                network.Dispose();
        }
    }
}
