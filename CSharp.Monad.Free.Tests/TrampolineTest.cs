using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharp.Monad.Tests
{
    [TestFixture]
    public class TrampolineTest
    {
        public static Free<F0, decimal> Fib(decimal n)
        {
            if (n < 2M) return FreeModule.Done<F0, decimal>(n);
            return from x in Trampoline.Suspend(() => Fib(n - 1))
                   from y in Trampoline.Suspend(() => Fib(n - 2))
                   select x + y;
        }

        [Test]
        public void TestFib()
        {
            Assert.That(Trampoline.Run<decimal, decimal, decimal>(Fib(2M)), Is.EqualTo(1M));
        }
    }
}
