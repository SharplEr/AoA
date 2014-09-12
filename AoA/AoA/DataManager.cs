using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IOData;
using VectorSpace;

namespace AoA
{
    public class DataManager
    {
        public static Tuple<SigmentData[], SigmentData[]> getShuffleFrom(FullData data, int m, double part)
        {
            //Первый пусть будет обучением, а второй контрольный
            Tuple<SigmentData[], SigmentData[]> ans = new Tuple<SigmentData[], SigmentData[]>(new SigmentData[m], new SigmentData[m]);

            int k = (int)Math.Round(data.Length * part);

            int[] learnDate = new int[data.Length - k];
            int[] controlDate = new int[k];

            for (int i = 0; i < data.Length - k; i++)
            {
                learnDate[i] = i;
            }

            for (int i = data.Length - k; i < data.Length; i++)
            {
                controlDate[i - data.Length + k] = i;
            }

            int j = 0;

            Random r = new Random(271828314);

            while (j < m)
            {
                int l, t;
                for (int i = 0; i < controlDate.Length; i++)
                {
                    l = r.Next(learnDate.Length - 1);
                    t = controlDate[i];
                    controlDate[i] = learnDate[l];
                    learnDate[l] = t;
                }

                SigmentData tempdata = new SigmentData(data, controlDate);

                //Не самый лучший способ исключить плохие разбиения.
                if (tempdata.GetResults().Equable > 1.5) continue;

                ans.Item2[j] = tempdata;
                ans.Item1[j] = new SigmentData(data, learnDate);

                j++;
            }

            return ans;
        }

        public static Tuple<SigmentData[], SigmentData[]>[] getShuffleFrom(FullData[] data, int m, double part)
        {
            Tuple<SigmentData[], SigmentData[]>[] ans = new Tuple<SigmentData[],SigmentData[]>[data.Length];

            for (int i = 0; i < data.Length; i++)
                ans[i] = getShuffleFrom(data[i], m, part);

            return ans;
        }
    }
}
