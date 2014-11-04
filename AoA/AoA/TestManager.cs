using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;
using IOData;
using Metaheuristics;
using VectorSpace;

namespace AoA
{
    /// <summary>
    /// Класс реализующий тестирование алгоритмов
    /// </summary>
    public class TestManager
    {
        Test[,] tests;
        int testing;
        int testingAlgorithm;
        int m;
        double part;
        FullData[] TestData;
        Type[] types;

        string fileName;

        [NonSerialized]
        BinaryFormatter deser = new BinaryFormatter();

        [NonSerialized]
        FileStream saveWriter;

        [NonSerialized]
        Parameter[][] parameters;

        [NonSerialized]
        Thread runer;

        public TestManager(FullData[] td, int m, double part, string name, Type[] ts, Parameter[][] ps)
        {
            this.m = m;
            this.part = part;
            TestData = td;
            types = ts;

            for (int i = 0; i < types.Length; i++ )
                if (!typeof(Algorithm).IsAssignableFrom(types[i])) throw new ArgumentException("Не верный тип, должен наследовать Algorithm");

            tests = new Test[TestData.Length, types.Length];

            testing = 0;
            testingAlgorithm = 0;
            fileName = name;

            Refresh(ps);

            if (!Save(saveWriter)) throw new SystemException();
        }

        public void Starting(Action<int, int, Type, int, int> w)
        {
            if (runer != null) throw new ArgumentException("Что-то уже работает!");
            runer = new Thread(() => Start(w));

            runer.Start();
        }


        protected void Start(Action<int, int, Type, int, int> w)
        {
            int startI = testing;
            int startJ = testingAlgorithm;

            for (int i = startI; i < TestData.Length; i++)
            {
                var td = DataManager.getShuffleFrom(TestData[i], m, part, new Random(271828314));

                for(int j = startJ; j< types.Length; j++)
                {
                    var finder = new FindAlgorithm(parameters[j], (x, y) => w(x, y, types[j], i, j), types[j], TestData[i], td.Item1, td.Item2);
                    var ans = finder.Find();
                    tests[i, j] = new Test(types[j], parameters[j], ans.Item2, ans.Item1);
                    testingAlgorithm = j++;
                }
                testingAlgorithm = 0;
                testing = i++;
            }
        }

        protected void MakeOutputFiles()
        {
            for (int i = 0; i<TestData.Length; i++)
                for (int j = 0; j < types.Length; j++)
                {
                    tests[i, j].Save(new StreamWriter(@types[j].ToString() + "("+ j.ToString() + ")at data №" + i.ToString() + ".txt", false));
                }
        }

        protected bool Save(Stream file)
        {
            try
            {
                deser.Serialize(file, this);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static TestManager GetFrom(Stream file, Parameter[][] ps)
        {
            TestManager ans = (TestManager)(new BinaryFormatter()).Deserialize(file);
            ans.Refresh(ps);
            return ans;
        }

        protected void Refresh(Parameter[][] ps)
        {
            deser = new BinaryFormatter();
            saveWriter = new FileStream(fileName, FileMode.Create);
            parameters = ps;

            if (testingAlgorithm == types.Length)
            {
                testing++;
                testingAlgorithm = 0;
            }
        }

        public void DisposeAndSave()
        {
            if (runer != null)
            {
                try
                {
                    runer.Abort();
                }
                catch
                { }
                runer = null;
            }

            Save(saveWriter);

            saveWriter.Close();
            saveWriter.Dispose();
            saveWriter = null;
        }

        public void Dispose()
        {
            if (saveWriter != null)
            {
                saveWriter.Close();
                saveWriter.Dispose();
            }

            if (runer != null)
            {
                try
                {
                    runer.Abort();
                }
                catch
                { }
                runer = null;
            }
        }
    }
}
