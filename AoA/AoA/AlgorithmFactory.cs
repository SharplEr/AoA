using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Metaheuristics;

namespace AoA
{
    public static class AlgorithmFactory
    {
        public static Func<object[], Algorithm> GetFactory(Type T)
        {
            if (!typeof(Algorithm).IsAssignableFrom(T)) throw new ArgumentException("Не верный тип, должен наследовать Algorithm");

            if (T.IsAbstract) throw new ArgumentException("Класс должен быть не абстрактным");

            return (x) => (Algorithm)Activator.CreateInstance(T, x);
        }

        public static Func<object[], Algorithm>[] GetFactory(params Type[] Ts)
        {
            if (Ts == null) throw new ArgumentNullException();

            Func<object[], Algorithm>[] ans = new Func<object[],Algorithm>[Ts.Length];

            for (int i = 0; i < ans.Length; i++)
                ans[i] = GetFactory(Ts[i]);

            return ans;
        }

        public static Type[] LoadFromDLL(string file)
        {
            Assembly a = Assembly.LoadFrom(file);
            List<Type> ans = new List<Type>();

            foreach (Type t in a.GetExportedTypes())
            {
                if (!t.IsAbstract && typeof(Algorithm).IsAssignableFrom(t))
                    ans.Add(t);
            }

            return ans.ToArray();
        }

        public static Type[] LoadFromDLL(string[] files)
        {
            List<Type> ans = new List<Type>();

            for (int i = 0; i < files.Length; i++)
                ans.AddRange(LoadFromDLL(files[i]));

            return ans.ToArray();
        }
    }
}
