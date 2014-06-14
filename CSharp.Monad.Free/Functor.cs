using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharp.Monad
{
    public interface Functor<F>
    {
        _1<F, B> Map<A, B>(Func<A, B> f, _1<F, A> fa);
    }
}
