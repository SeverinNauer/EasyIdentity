using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyIdentity.Core
{
    public abstract class TypeWrapper<T>
    {
        public T Value { get; }

        protected TypeWrapper(T value) => Value = value;

        public static implicit operator T(TypeWrapper<T> wrapper) => wrapper.Value;
    }
}
