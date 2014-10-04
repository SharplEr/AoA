using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IOData;
using VectorSpace;

namespace AoA
{
    /// <summary>
    /// Набор методов для перемешивания данных
    /// </summary>
    public class DataManager
    {
        /// <summary>
        /// Возвращает перемешиваные данные, разбитые на контрольную выборку и на обучающую
        /// </summary>
        /// <param name="data">Данные</param>
        /// <param name="m">Число перестановок</param>
        /// <param name="part">Доля идущая на контроль</param>
        public static Tuple<SigmentData[], SigmentData[]> getShuffleFrom(FullData data, int m, double part, Random r)
        {
            //Первый пусть будет обучением, а второй контрольный
            Tuple<SigmentData[], SigmentData[]> ans = new Tuple<SigmentData[], SigmentData[]>(new SigmentData[m], new SigmentData[m]);

            int k = (int)Math.Round(data.Length * part);
            if (k < 1) k = 1;
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

                ans.Item2[j] = new SigmentData(data, (int[])controlDate.Clone());

                //Не самый лучший способ исключить плохие разбиения.
                if (ans.Item2[j].GetResults().Equable > 1.5) continue;

                ans.Item1[j] = new SigmentData(data, (int[])learnDate.Clone());
                j++;
            }

            return ans;
        }

        //Возможно эта функция не понадобится. В конце концов слишком уж много будет.
        public static Tuple<SigmentData[], SigmentData[]>[] getShuffleFrom(FullData[] data, int m, double part)
        {
            Tuple<SigmentData[], SigmentData[]>[] ans = new Tuple<SigmentData[],SigmentData[]>[data.Length];

            for (int i = 0; i < data.Length; i++)
                ans[i] = getShuffleFrom(data[i], m, part, new Random(271828314));

            return ans;
        }
    }
}