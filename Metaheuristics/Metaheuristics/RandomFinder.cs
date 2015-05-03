﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Metaheuristics
{
    
    public abstract class RandomFinder<T> : FinderCustom<T> where T : IQuality<T>
    {
        int maxStep;

        Random random = new Random();

        public RandomFinder(Parameter[] p, Action<int, int> w, int mxstep)
            : base(p, w)
        {
            maxStep = mxstep;

            starter = new RandomStart(random, p);

            stoper = new Stoper(() => step, maxStep);
            
            jumper = new TrueJumper<T>();

            neighbor = new RandomNeighborhood(random, p);
        }

    }
}
