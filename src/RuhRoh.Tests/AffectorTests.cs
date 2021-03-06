﻿using System;
using RuhRoh.Affectors;
using RuhRoh.Triggers;
using RuhRoh.Triggers.Internal;
using Xunit;
using Xunit.Sdk;

namespace RuhRoh.Tests
{
    public class AffectorTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        public void Delayer_Should_Delay_With_The_Given_Time(int seconds)
        {
            var affector = new Delayer(TimeSpan.FromSeconds(seconds));

            var t = new ExecutionTimer();
            t.Aggregate(() => affector.Affect());

            Assert.InRange(t.Total, seconds - 1, seconds + 1);
        }

        [Fact]
        public void ExceptionThrower_Should_Throw_An_Exception()
        {
            var exception = new InvalidOperationException();
            var affector = new ExceptionThrower(exception);

            var e = Assert.Throws<InvalidOperationException>(() => affector.Affect());
            Assert.Equal(e, exception);
        }

        [Fact]
        internal void And_Should_Combine_Triggers()
        {
            var exception = new InvalidOperationException();
            var affector = new ExceptionThrower(exception);

            affector.AtRandom().And.AfterNCalls(2);

            var triggers = ((IAffector)affector).Triggers;
            Assert.Single(triggers);
            Assert.IsType<CombinedTrigger>(triggers[0]);

            var combinedTrigger = (CombinedTrigger)triggers[0];
            Assert.IsType<RandomTrigger>(combinedTrigger.First);
            Assert.IsType<TimesCalledTrigger>(combinedTrigger.Second);
            Assert.Equal(Logical.And, combinedTrigger.Operand);
        }

        [Fact]
        internal void Or_Should_Combine_Triggers()
        {
            var exception = new InvalidOperationException();
            var affector = new ExceptionThrower(exception);

            affector.AtRandom().Or.AfterNCalls(2);

            var triggers = ((IAffector)affector).Triggers;
            Assert.Single(triggers);
            Assert.IsType<CombinedTrigger>(triggers[0]);

            var combinedTrigger = (CombinedTrigger)triggers[0];
            Assert.IsType<RandomTrigger>(combinedTrigger.First);
            Assert.IsType<TimesCalledTrigger>(combinedTrigger.Second);
            Assert.Equal(Logical.Or, combinedTrigger.Operand);
        }

        [Fact]
        internal void Xor_Should_Combine_Triggers()
        {
            var exception = new InvalidOperationException();
            var affector = new ExceptionThrower(exception);

            affector.AtRandom().Xor.AfterNCalls(2);

            var triggers = ((IAffector)affector).Triggers;
            Assert.Single(triggers);
            Assert.IsType<CombinedTrigger>(triggers[0]);

            var combinedTrigger = (CombinedTrigger)triggers[0];
            Assert.IsType<RandomTrigger>(combinedTrigger.First);
            Assert.IsType<TimesCalledTrigger>(combinedTrigger.Second);
            Assert.Equal(Logical.Xor, combinedTrigger.Operand);
        }

        [Fact]
        internal void InvalidCombination_Should_Throw_InvalidOperationException()
        {
            var exception = new InvalidOperationException();
            var affector = new ExceptionThrower(exception);

            Assert.Throws<InvalidOperationException>(() => affector.AtRandom().And.Or.AfterNCalls(2));
        }

        [Fact]
        internal void AndOrXor_Should_Throw_InvalidOperationException_When_Applied_Without_Previous_Trigger()
        {
            var exception = new InvalidOperationException();
            var affector = new ExceptionThrower(exception);

            Assert.Throws<InvalidOperationException>(() => affector.And.AfterNCalls(2));
        }

        [Fact]
        internal void Not_Should_Inverse_Trigger()
        {
            var exception = new InvalidOperationException();
            var affector = new ExceptionThrower(exception);

            affector.Not.AfterNCalls(2);

            var triggers = ((IAffector)affector).Triggers;
            Assert.Single(triggers);
            Assert.IsType<CombinedTrigger>(triggers[0]);

            var combinedTrigger = (CombinedTrigger)triggers[0];
            Assert.Null(combinedTrigger.First);
            Assert.IsType<TimesCalledTrigger>(combinedTrigger.Second);
            Assert.Equal(Logical.Not, combinedTrigger.Operand);
        }

        [Fact] internal void Not_Should_Inverse_CombinedTrigger()
        {
            var exception = new InvalidOperationException();
            var affector = new ExceptionThrower(exception);

            affector.AtRandom().And.Not.AfterNCalls(2);

            var triggers = ((IAffector)affector).Triggers;
            Assert.Single(triggers);
            Assert.IsType<CombinedTrigger>(triggers[0]);

            var combinedTrigger = (CombinedTrigger)triggers[0];
            Assert.IsType<RandomTrigger>(combinedTrigger.First);
            Assert.IsType<CombinedTrigger>(combinedTrigger.Second);
            Assert.Equal(Logical.And, combinedTrigger.Operand);

            combinedTrigger = (CombinedTrigger)combinedTrigger.Second;
            Assert.Null(combinedTrigger.First);
            Assert.IsType<TimesCalledTrigger>(combinedTrigger.Second);
            Assert.Equal(Logical.Not, combinedTrigger.Operand);
        }
    }
}