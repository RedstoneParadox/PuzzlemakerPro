using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzlemakerPro.Scripts.Util
{
    class Maybe<T>
    {
        private readonly bool empty;
        private readonly T value;

        private Maybe()
        {
            empty = true;
            value = default;
        }

        private Maybe(T value)
        {
            this.empty = false;
            this.value = value;
        }

        public Maybe<T> Some(T value)
        {
            return new Maybe<T>(value);
        }

        public Maybe<T> None()
        {
            return new Maybe<T>();
        }

        public 
    }
}
