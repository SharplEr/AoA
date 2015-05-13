using System;
using IOData;

namespace AoA
{
    public class SigmentData: SigmentInputData
    {
        protected int[] indexer;
        public SigmentData(FullData d, int[] ind)
            : base(d, ind)
        {
            indexer = ind;
        }

        public Results GetResults()
        {

            return data.Output;
        }

        public Double[] GetDoubles()
        {
            return data.Regression;
        }

        public void AddControlError(Results rs, Info[] info)
        {
            if (indexer.Length != Length) throw new ArgumentOutOfRangeException();

            for (int i = 0; i < Length; i++)
                if (rs[i] == data.Output[i]) info[indexer[i]].errorControl.Add(0);
                else info[indexer[i]].errorControl.Add(1);
        }

        public void AddLearnError(Results rs, Info[] info)
        {
            if (indexer.Length != Length) throw new ArgumentOutOfRangeException();

            for (int i = 0; i < indexer.Length; i++)
                if (rs[i] == data.Output[i]) info[indexer[i]].errorLearn.Add(0);
                else info[indexer[i]].errorLearn.Add(1);
        }

        public void AddControlError(Double[] rs, Info[] info)
        {
            if (indexer.Length != Length) throw new ArgumentOutOfRangeException();

            for (int i = 0; i < Length; i++)
                info[indexer[i]].errorControl.Add(Math.Abs(data.Regression[i] - rs[i]));
        }

        public void AddLearnError(Double[] rs, Info[] info)
        {
            if (indexer.Length != Length) throw new ArgumentOutOfRangeException();

            for (int i = 0; i < indexer.Length; i++)
                info[indexer[i]].errorLearn.Add(Math.Abs(data.Regression[i] - rs[i]));
        }

        public int[] GetMaxDiscretePath()
        {
            //Только дискретной части? А после преобразования?
            return data.maxdiscretePart;
        }
    }
}