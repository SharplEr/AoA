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
    public abstract class TestManager
    {
        //Тесты уже взятые из файлов
        FullData[] TestData;

        //Взяли из файлов да ещё и перемешали
        SigmentData[][] TestDataSigment;    //Такое может и в память не вместиться

        Test[] tests;

        bool canSave = false;

        //[NonSerialized]
        BinaryFormatter deser = new BinaryFormatter();

        public bool CanSave
        {
            get
            { return canSave; }
        }

        protected bool SaveData(Stream s)
        {
            try
            {
                deser.Serialize(s, new Tuple<FullData[], SigmentData[][]>(TestData, TestDataSigment));
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected bool LoadData(Stream s)
        {
            try
            {
                Tuple<FullData[], SigmentData[][]> temp = (Tuple<FullData[], SigmentData[][]>)deser.Deserialize(s);
                TestData = temp.Item1;
                TestDataSigment = temp.Item2;
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
         * */
        protected bool SaveTest(Stream s)
        {
            try
            {
                //deser.Serialize(s, new Tuple<FullData[], SigmentData[][]>(TestData, TestDataSigment));
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
