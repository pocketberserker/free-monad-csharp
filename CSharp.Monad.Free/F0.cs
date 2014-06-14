using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharp.Monad
{
    class F0FunctorImpl : Functor<F0>
    {
        internal F0FunctorImpl() { }

        public _1<F0, B> Map<A, B>(Func<A, B> f, _1<F0, A> fa)
        {
 	          return F0.Wrap(() => f((fa as F0<A>).Apply()));
        }
    }

    internal class F0Impl<A> : F0<A>
    {
        private readonly Func<A> f;
        public F0Impl(Func<A> f)
        {
            this.f = f;
        }
        public A Apply()
        {
            return f();
        }
    }

    public sealed class F0
    {
        public static readonly Functor<F0> Functor = new F0FunctorImpl();

        public static F0<A> Wrap<A>(Func<A> f)
        {
            return new F0Impl<A>(f);
        }

        private F0() {}
    }

    public interface F0<A> : _1<F0, A>
    {
        A Apply();
    }
}
