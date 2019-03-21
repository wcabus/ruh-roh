﻿using Castle.DynamicProxy;
using System;
using System.Threading.Tasks;

namespace RuhRoh.Affectors
{
    internal class Delayer : Affector
    {
        private readonly TimeSpan _delay;

        public Delayer(TimeSpan delay)
        {
            _delay = delay;
        }

        protected internal sealed override void Affect(IInvocation invocation)
        {
            Task.Delay(_delay).GetAwaiter().GetResult();
        }
    }
}
