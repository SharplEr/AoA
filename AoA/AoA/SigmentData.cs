using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IOData;
using VectorSpace;
using ArrayHelper;
using System.Collections.Concurrent;

namespace AoA
{
    public class SigmentData: SigmentInputData
    {   
        public SigmentData(FullData d, int[] ind): base(d, ind) {}

        public Results GetResults()
        {
            return new Results((i) => data.Output[indexer[i]], Length);
        }

        public void AddControlError(Results rs, Info[] info)
        {
            for (int i = 0; i < indexer.Length; i++)
                if (rs[i] == data.Output[indexer[i]]) info[indexer[i]].errorControl.Add(0);
                else info[indexer[i]].errorControl.Add(1);
        }

        public void AddLearnError(Results rs, Info[] info)
        {
            for (int i = 0; i < indexer.Length; i++)
                if (rs[i] == data.Output[indexer[i]]) info[indexer[i]].errorLearn.Add(0);
                else info[indexer[i]].errorLearn.Add(1);
        }
    }
}
