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
        protected List<Tuple<Type, Test[]>> info;

        [NonSerialized]
        protected List<Parameter[]> parameters;

        protected bool haveChange;

        protected int m;
        protected double part;
        protected FullData[] TestData;

        protected string fileName;

        [NonSerialized]
        BinaryFormatter deser = new BinaryFormatter();

        [NonSerialized]
        FileStream saveWriter;

        [NonSerialized]
        Thread runer;

        public TestManager(FullData[] td, int m, double part, string name, Type[] ts, Parameter[][] ps)
        {
            this.m = m;
            this.part = part;
            TestData = td;
            info = new List<Tuple<Type, Test[]>>();

            for (int i = 0; i < ts.Length; i++)
                if (!typeof(Algorithm).IsAssignableFrom(ts[i])) throw new ArgumentException("Не верный тип! Должен наследовать Algorithm.");

            for (int i = 0; i < ts.Length; i++)
                info.Add(new Tuple<Type, Test[]>(ts[i], new Test[td.Length]));
            
            fileName = name;

            Refresh(ps);

            haveChange = false;

            if (!Save(saveWriter)) throw new SystemException("Не удается сохранить");
        }

        /*
        protected Tuple<int, double, > LoadConfig(StreamReader reader)
        {
            string s = reader.ReadLine();

            
        }*/

        /*
        public TestManager(FullData[] td, int m, double part, string name, Type[] ts, Parameter[][] ps)
        {
            this.m = m;
            this.part = part;
            TestData = td;
            info = new List<Tuple<Type, Test[]>>();

            for (int i = 0; i < ts.Length; i++)
                if (!typeof(Algorithm).IsAssignableFrom(ts[i])) throw new ArgumentException("Не верный тип! Должен наследовать Algorithm.");

            for (int i = 0; i < ts.Length; i++)
                info.Add(new Tuple<Type, Test[]>(ts[i], new Test[td.Length]));

            fileName = name;

            Refresh(ps);

            haveChange = false;

            if (!Save(saveWriter)) throw new SystemException("Не удается сохранить");
        }*/

        public TestManager()
        {
            
        }

        public void Starting(Action<int, int, Type, int, int> w)
        {
            if (runer != null) throw new ArgumentException("Что-то уже работает!");
            runer = new Thread(() => Start(w));

            runer.Start();
        }

        public void Add(Type t, Parameter[] ps)
        {
            info.Add(new Tuple<Type, Test[]>(t, new Test[TestData.Length]));
            parameters.Add(ps);
        }

        public void Add(Type[] t, Parameter[][] ps)
        {
            for (int i = 0; i < t.Length; i++)
                Add(t[i], ps[i]);
        }

        public void Pause()
        {
            runer.Suspend();
        }

        public void Resume()
        {
            runer.Resume();
        }

        protected void Start(Action<int, int, Type, int, int> w)
        {
            int i = 0;
            while(i < TestData.Length)
            {
                var td = DataManager.GetShuffleFrom(TestData[i], m, part, new Random(271828314));

                for(int j = 0; j< info.Count; j++)
                {
                    if (info[j].Item2[i] == null) continue;

                    var finder = new FindAlgorithm(parameters[j], (x, y) => w(x, y, info[j].Item1, i, j), info[j].Item1, TestData[i], td.Item1, td.Item2);
                    var ans = finder.Find();
                    info[j].Item2[i] = new Test(info[j].Item1, parameters[j], ans.Item2, ans.Item1);
                }
                i++;
                if ((i == TestData.Length) && haveChange) i = 0;
            }
        }

        /*
         * Можно добавить генерирование отчетов в виде tex файла, лал-с:)
         */
        protected void MakeOutputFiles()
        {
            for (int i = 0; i<info.Count; i++)
                for (int j = 0; j < info[i].Item2.Length; j++)
                    info[i].Item2[j].Save(new StreamWriter(@info[i].Item1.ToString() + "(" + j.ToString() + ")at data №" + i.ToString() + ".txt", false));
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
            parameters = ps.ToList();
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