using System;
using IOData;

namespace AoA
{
    /// <summary>
    /// Набор методов для перемешивания данных
    /// </summary>
    public static class DataManager
    {
        /// <summary>
        /// Возвращает перемешиваные данные, разбитые на контрольную выборку и на обучающую
        /// </summary>
        /// <param name="data">Данные</param>
        /// <param name="m">Число перестановок</param>
        /// <param name="part">Доля идущая на контроль. Если число примеров идущих на контроль будет меньше или равна 1, будет реализован скользящий контроль с одним оделяемым объектом</param>
        public static Tuple<SigmentData[], SigmentData[]> GetShuffleFrom(FullData data, int m, double part, Random r)
        {
            if (data==null) throw new ArgumentException("data is null");
            if (r == null) throw new ArgumentException("r is null");

            //Число на контроле
            int k = (int)Math.Round(data.Length * part);
            if (k <= 1) return GetOneFrom(data);

            if (m < 1) return GetNoCross(data, data.Length - k);

            //Первый пусть будет обучением, а второй контрольный
            Tuple<SigmentData[], SigmentData[]> ans = new Tuple<SigmentData[], SigmentData[]>(new SigmentData[m], new SigmentData[m]);

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

                //Console.WriteLine("Хуй {0}. j = {1}", ans.Item2[j].GetResults().Equable, j);

                //Не самый лучший способ исключить плохие разбиения.
                if (ans.Item2[j].GetResults().Equable > 2.2)
                    continue;

                ans.Item1[j] = new SigmentData(data, (int[])learnDate.Clone());
                j++;
            }

            return ans;
        }

        //Возможно эта функция не понадобится. В конце концов слишком уж много будет.
        public static Tuple<SigmentData[], SigmentData[]>[] GetShuffleFrom(FullData[] data, int m, double part)
        {
            if (data == null) throw new ArgumentException("data is null");

            Tuple<SigmentData[], SigmentData[]>[] ans = new Tuple<SigmentData[],SigmentData[]>[data.Length];

            for (int i = 0; i < data.Length; i++)
                ans[i] = GetShuffleFrom(data[i], m, part, new Random(271828314));

            return ans;
        }

        public static Tuple<SigmentData[], SigmentData[]> GetOneFrom(FullData data)
        {
            if (data == null) throw new ArgumentException("data is null");

            Tuple<SigmentData[], SigmentData[]> ans = new Tuple<SigmentData[], SigmentData[]>(new SigmentData[data.Length], new SigmentData[data.Length]);

            for (int i = 0; i < data.Length; i++)
            {
                ans.Item1[i] = new SigmentData(data, Statist.GetIndex(data.Length - 1, i));
                ans.Item2[i] = new SigmentData(data, new int[]{i});
            }

            return ans;
        }

        public static Tuple<SigmentData[], SigmentData[]> GetNoCross(FullData data, int n)
        {
            return new Tuple<SigmentData[], SigmentData[]>(
                    new SigmentData[] { new SigmentData(data, Statist.GetIndex(n)) },
                    new SigmentData[] { new SigmentData(data, Statist.GetIndexFromTo(n, data.Length)) }
                    );
        }
    }
}