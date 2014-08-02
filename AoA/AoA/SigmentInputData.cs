using IOData;
using VectorSpace;
using ArrayHelper;

namespace AoA
{
    public class SigmentInputData
    {
        protected FullData data;
        protected int[] indexer;

        public int Length
        {
            get
            {
                return indexer.Length;
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
            data = d;
            indexer = ind;
        }

        public int[][] GetDiscreteArray()
        {
            int[][] ans = new int[Length][];
            
            for (int i = 0; i < Length; i++)
                ans[i] = data.DiscreteInput[indexer[i]].CloneOk<int[]>();
            
            return ans;
        }

        public Vector[] GetСontinuousArray()
        {
            Vector[] ans = new Vector[Length];

            for (int i = 0; i < Length; i++)
                ans[i] = data.СontinuousInput[indexer[i]].CloneOk();

            return ans;
        }

        public MixData[] GetMixArray()
        {
            MixData[] ans = new MixData[Length];

            for (int i = 0; i < Length; i++)
                ans[i] = data.MixInput[indexer[i]].CloneOk();

            return ans;
        }
    }
}