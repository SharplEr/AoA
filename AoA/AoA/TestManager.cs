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
    public class TestManager
    {
        //Тесты уже взятые из файлов
        FullData[] TestData;

        //Взяли из файлов да ещё и перемешали
        Tuple<SigmentData[], SigmentData[]>[] TestDataSigment;    //Такое может и в память не вместиться

        Test[] tests;
        int testing;

        bool canSaveData = false;
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

        protected void Start(Func<object[], Algorithm> getAlg)
        {
            
            //Перед вызовом надо обязателньо найти последний выполненный тест!
            for (int i = testing; i < tests.Length; i++)
            {
                //Тут надо создавать элементы класса хуй: FinderAlg, которые определяют лучший object для каждого заданьица.
                //Он получает Func<object[], Algorithm> на вход и массив сигментов данных, возвращает object[] , и лог.
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

        /*
         *Нихуя бля вообще
         *Короче
         *делаем принт лога еба, короче, с параметрами ебать, при которых это хуярит-с. И название ещё можно какое-нить заебенить)
         *Такие пироги нормально зайдут.
         *
         * Ок, да, а хранить как? как востанавливать, ты об этом подумал, хуярий, вообще, а? Гений блядь.
         * */
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
