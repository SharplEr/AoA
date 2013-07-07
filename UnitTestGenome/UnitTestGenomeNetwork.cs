using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Araneam;
using VectorSpace;
using GenomeNeuralNetwork;
using MyParallel;
using System.IO;
using System.Threading;


namespace UnitTestGenome
{
    [TestClass]
    public class UnitTestGenomeNetwork
    {
        [TestMethod]
        public void TestGenomeBaseLoad()
        {
            const string name = @".\xyu.csv";
            const string some1 = @"Пол,Возраст,Диагноз,стадия ГБ, Национальность,Ожирение,Сахарный диабет,Курение,Алкоголь,FV,FII,AGTR,AGT_174,AGT_235,PAI1,MTHFR,ACE,NOS,APOE,LPL+73in6,LPL+82in6,LPL_HindIII,LPL_S447X,LIPC(-514),LIPCV155V,CETP_taq,CETP_I405V";
            const string some2 = @"М,""64,5"",ГБ,2,русский,0,0,0,0,GG,GG,AC,CT,CT,45,CC,II,GG,34,TT,TT,AA,CC,GG,CC,AG,AA";
            const string some3 = @"М,41,ГБ,2,русский,2,0,0,0,GG,GG,AA,CC,CT,45,CT,DD,GT,33,GT,GG,AC,GC,AG,CA,AA,AG";
            StreamWriter writer = new StreamWriter(name);

            writer.WriteLine(some1);
            writer.WriteLine(some2);
            writer.WriteLine(some3);
            writer.Close();

            CSVReader reader = new CSVReader(name);
            Double[] v1 = new Double[] { 64.5, 0, 0, 0, 1.8, 1.8, 1+1.0/35, 1+11.0 / 35.0, 1+11.0 / 35.0, 45, 1+0.2,1+ 0.6, 1+0.8, 34, 1+1.0, 1+1.0, 1+0.0, 1+0.2,1+ 0.8,1+ 0.2,1+ 4.0/35.0, 1+0.0 ,1+ 0.5};

            Vector[] date = null;

            date = GenomeNetwork.LoadTestDate(reader);

            Assert.AreEqual(v1.Length, date[0].Length, "Размерности не совпадают");

            for (int i = 0; i < v1.Length; i++)
            {
                Assert.AreEqual(v1[i], date[0][i], 0.00001,"Не совпал {0}-й элемент");
            }
        }

        [TestMethod]
        public void TestGenomeLearn()
        {
            const string name = @".\xyu.csv";
            const string some1 = @"Пол,Возраст,Диагноз,стадия ГБ, Национальность,Ожирение,Сахарный диабет,Курение,Алкоголь,FV,FII,AGTR,AGT_174,AGT_235,PAI1,MTHFR,ACE,NOS,APOE,LPL+73in6,LPL+82in6,LPL_HindIII,LPL_S447X,LIPC(-514),LIPCV155V,CETP_taq,CETP_I405V";
            const string some2 = @"М,64,ГБ,2,русский,0,0,0,0,GG,GG,AC,CT,CT,45,CC,II,GG,34,TT,TT,AA,CC,GG,CC,AG,AA";
            const string some3 = @"М,55,ГБ,1,русский,1,0,0,0,AG,GG,AC,CC,CT,45,CC,DD,GG,23,TT,TT,AA,CC,GG,CA,AG,AA";

            StreamWriter writer = new StreamWriter(name);

            writer.WriteLine(some1);
            writer.WriteLine(some2);
            writer.WriteLine(some3);
            writer.Close();

            double e = 10;

            new Thread(() =>
                {
                    try
                    {
                        GenomeNetwork nw = new GenomeNetwork(0.5, 1000);

                        nw.Reload(new string[]{name});

                        e = nw.FullLearn(0.3);
                    }
                    catch(SystemException exp)
                    {
                        Console.WriteLine("Сообщение: {0}, Стек: {1}", exp.Message, exp.StackTrace);
                    }
                }).InMTA();

            Assert.AreEqual(0, e, 0.3,"Не обучается");
        }
    }
}
