using System;
using VectorSpace;
using MyParallel;
using ArrayHelper;

namespace AoA
{
    public class SpaceInfo
    {
        public Vector Center;
        public double MaxDistanceOfCenter;
        public double AvgDistanceOfCenter;

        Vector[] vectors;

        public SpaceInfo(Vector[] vs)
        {
            vectors = vs;
            CalcStat();
        }

        protected void CalcStat()
        {
            Center = vectors[0].CloneOk();

            for (int i = 1; i < vectors.Length; i++)
                Center.Addication(vectors[i]);
            Center.Multiplication(1.0 / vectors.Length);

            MaxDistanceOfCenter = 0.0;
            double d=0.0;
            for (int i = 0; i < vectors.Length; i++)
            { 
                double t = Math.Sqrt((double)(Center-vectors[i]));
                d+=t;
                if (MaxDistanceOfCenter < t) MaxDistanceOfCenter = t;
            }
            AvgDistanceOfCenter = d / vectors.Length;
        }

        public Triple Distance(Vector v)
        {
            Triple t;
            t.Max = 0;
            t.Min = Double.MaxValue;
            t.Avg = Math.Sqrt((double)(Center-v));
            for (int i = 0; i < vectors.Length; i++)
            {
                double d = Math.Sqrt((double)(v - vectors[i]));
                if (t.Max < d) t.Max = d;
                if (t.Min > d) t.Min = d;
            }
            return t;
        }
    }
}
