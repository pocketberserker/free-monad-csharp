using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LangExt;
using System.ComponentModel;

namespace CSharp.Monad
{
    public static class FreeModule
    {
        public static Free<G, B> Done<G, B>(B b)
        {
            return new Done<G, B>(b);
        }

        public static Free<G, B> LiftF<G, B>(_1<G, B> value, Functor<G> g)
        {
            return new Suspend<G, B>(g.Map<B, Free<G, B>>(a => new Done<G, B>(a), value));
        }

        public static Free<G, B> Suspend<G, B>(_1<G, Free<G, B>> b)
        {
            return new Suspend<G, B>(b);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Free<G, V> SelectMany<G, T, U, V>(this Free<G, T> self, Func<T, Free<G, U>> f, Func<T, U, V> g)
        {
            return self.SelectMany(x => f(x).SelectMany(y => Done<G, V>(g(x, y))));
        }
    }

    public abstract class Free<F, A>
    {
        internal Free() { }

        public abstract Free<F, B> SelectMany<B>(Func<A, Free<F, B>> f);

        public Free<F, B> Select<B>(Func<A, B> f)
        {
            return SelectMany(a => new Done<F, B>(f(a)));
        }

        private static Choice<_1<F, Free<F, A>>, A> Resume<X1, X2>(Free<F, A> current, Functor<F> f)
        {
            var tmp = current;
            while (true)
            {
                var d = tmp as Done<F, A>;
                if (d != null)
                {
                    return Choice.Create<_1<F, Free<F, A>>, A>(d.Value);
                }
                var s = tmp as Suspend<F, A>;
                if (s != null)
                {
                    return Choice.Create<_1<F, Free<F, A>>, A>(s.Value);
                }
                var gosub1 = tmp as Gosub<F, X1, A>;
                var dd = gosub1.Value as Done<F, X1>;
                if (dd != null)
                {
                    tmp = gosub1.Func(dd.Value);
                    continue;
                }
                var ss = gosub1.Value as Suspend<F, X1>;
                if (ss != null)
                {
                    return Choice.Create<_1<F, Free<F, A>>, A>(f.Map(o => o.SelectMany(gosub1.Func), ss.Value));
                }
                var gosub2 = gosub1.Value as Gosub<F, X2, X1>;
                tmp = gosub2.Value.SelectMany(o => gosub2.Func(o).SelectMany(gosub1.Func));
            }
        }
        public Choice<_1<F, Free<F, A>>, A> Resume<X1, X2>(Functor<F> f)
        {
            return Resume<X1, X2>(this, f);
        }

        public A Go<X1, X2>(Func<_1<F, Free<F, A>>, Free<F, A>> f, Functor<F> g)
        {
            var current = this;
            while (true)
            {
                var ret = current.Resume<X1, X2>(g).Match(
                    l => Tuple.Create(false,(object) f(l)),
                    r => Tuple.Create(true,(object) r));
                if (ret._1()) return (A)ret._2();
                current = (Free<F, A>)ret._2();
            }
        }
    }

    sealed class Done<F, A> : Free<F, A>
    {
        public A Value { get; private set; }

        internal Done(A a)
        {
            Value = a;
        }

        public override Free<F, B> SelectMany<B>(Func<A, Free<F, B>> f)
        {
            return new Gosub<F, A, B>(this, f);
        }
    }

    sealed class Suspend<F, A> : Free<F, A>
    {
        public _1<F, Free<F, A>> Value { get; private set; }

        internal Suspend(_1<F, Free<F, A>> a)
        {
            Value = a;
        }

        public override Free<F, B> SelectMany<B>(Func<A, Free<F, B>> f)
        {
            return new Gosub<F, A, B>(this, f);
        }
    }

    sealed class Gosub<F, A, B> : Free<F, B>
    {
        public Free<F, A> Value { get; private set; }
        public Func<A, Free<F, B>> Func { get; private set; } 

        internal Gosub(Free<F, A> a, Func<A, Free<F, B>> f)
        {
            Value = a;
            Func = f;
        }

        public override Free<F, C> SelectMany<C>(Func<B, Free<F, C>> g)
        {
            return new Gosub<F, A, C>(Value, aa => new Gosub<F, B, C>(Func(aa), g));
        }
    }
}
