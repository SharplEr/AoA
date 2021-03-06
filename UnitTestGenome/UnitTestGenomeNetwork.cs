﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Araneam;
using VectorSpace;
using GenomeNeuralNetwork;
using MyParallel;
using System.IO;
using System.Threading;
using IODate;


namespace UnitTestGenome
{
    [TestClass]
    public class UnitTestGenomeNetwork
    {
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

                        e = nw.FullLearn(0.3).Error;
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
