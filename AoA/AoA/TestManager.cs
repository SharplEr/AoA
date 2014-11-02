using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using IOData;
using Metaheuristics;
using VectorSpace;

namespace AoA
{
    /// <summary>
    /// Класс реализующий тестирование алгоритмов
    /// </summary>
    /*
     * Надо переписать, принимать набор алгоритмов и тестировать их на перемешиваниях, потом на других и т.д. А то памяти на запасешься. 
    */ 
    public class TestManager
    {
        //Тесты уже взятые из файлов
        FullData[] TestData;

        //Взяли из файлов да ещё и перемешали
        Tuple<SigmentData[], SigmentData[]>[] TestDataSigment;    //Такое может и в память не вместиться - да,да.

        Test[] tests;
        int testing;

        /// <summary>
        /// Можно ли сохранить данные?
        /// </summary>
        bool canSaveData = false;
        /// <summary>
        /// Можно ли сохранить прогресс выполненных тестов?
        /// </summary>
        bool canSaveTest = false;

        [NonSerialized]
        BinaryFormatter deser = new BinaryFormatter();
        [NonSerialized]
        FileStream testStream;

        public TestManager(FullData[] td, int m, double part, string nameForData, string nameForDataSigment, string nameForTest)
        {
            //Тестовые данные
            TestData = td;
            //Мешаем
            TestDataSigment = DataManager.getShuffleFrom(TestData, m, part);
           
            //Можно сохранить данные
            canSaveData = true;

            FileStream sw1 = new FileStream(nameForData, FileMode.Create);
            FileStream sw2 = new FileStream(nameForDataSigment, FileMode.Create);
            if (!SaveData(sw1, sw2)) throw new SystemException();
            sw1.Close();
            sw1.Dispose();
            sw2.Close();
            sw2.Dispose();

            //Подготавливаем план тестов
            tests = new Test[TestData.Length];
            //Можно сохранить
            canSaveTest = true;
            testStream = new FileStream(nameForTest, FileMode.Create);
            if (!SaveTest()) throw new SystemException();
            //Начинаем первый тест
            testing = 0;
        }

        protected void Start(Parameter[] p, Func<object[], Algorithm> getAlg, Action<int, int> w)
        {
            //Перед вызовом надо обязателньо найти последний выполненный тест!
            for (; testing < tests.Length; testing++)
            {
                FindAlgorithm finder = new FindAlgorithm(p, w, getAlg, TestData[testing], TestDataSigment[testing].Item1, TestDataSigment[testing].Item2);

                var o = finder.Find();
                tests[testing].parameter = o.Item1;
                tests[testing].log = o.Item2;
                tests[testing].ready = true;
            }
        }

        public bool CanSaveData
        {
            get
            { return canSaveData; }
        }

        public bool CanSaveTest
        {
            get
            { return canSaveTest; }
        }

        protected bool SaveData(Stream data, Stream sigmentData)
        {
            try
            {
                if (!canSaveData) return false;
                deser.Serialize(data, TestData);
                deser.Serialize(sigmentData, TestDataSigment);
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected bool LoadData(Stream data, Stream sigmentData)
        {
            try
            {
                FullData[] TestData = (FullData[])deser.Deserialize(data);
                Tuple<SigmentData[], SigmentData[]>[] TestDataSigment = (Tuple<SigmentData[], SigmentData[]>[])deser.Deserialize(sigmentData);
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected bool SaveTest()
        {
            try
            {
                if (!canSaveTest) return false;
                deser.Serialize(testStream, tests);
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected bool LoadTest(Stream s)
        {
            try
            {
                tests = (Test[]) deser.Deserialize(s);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Refresh()
        {
            
        }
    }
}
