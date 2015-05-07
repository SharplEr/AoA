using IOData;
using VectorSpace;

namespace AoA
{
    public class SigmentInputData
    {
        protected FullData data;

        public int Length
        {
            get
            {
                return data.Length;
            }
        }

        public int Dimension
        {
            get
            {
                return data.Dimension;
            }
        }

        public SigmentInputData(FullData d, int[] ind)
        {
            data = new FullData(d, ind);
        }

        public int[][] GetDiscreteArray()
        {
            return data.DiscreteInput;
        }

        public Vector[] GetСontinuousArray()
        {
            return data.СontinuousInput;
        }

        public MixData[] GetMixArray()
        {
            return data.MixInput;
        }
    }
}