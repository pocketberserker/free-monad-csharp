using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharp.Monad
{
    public static class Trampoline
    {
        public static Free<F0, A> Delay<A>(Func<A> f)
        {
            return FreeModule.Suspend<F0, A>(F0.Wrap(() => FreeModule.Done<F0, A>(f())));
        }

        public static Free<F0, A> Suspend<A>(Func<Free<F0, A>> f)
        {
            return FreeModule.Suspend(F0.Wrap(f));
        }

        public static A Run<X1, X2, A>(Free<F0, A> f)
        {
            return f.Go<X1, X2>(a => (a as F0<Free<F0, A>>).Apply(), F0.Functor);
        }
    }
}
