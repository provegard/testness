using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TestNess.Lib.Test
{
    public static class Signature
    {
        public static Sig Of(MethodInfo mi)
        {
            return new Sig(mi);
        }

        public class Sig
        {
            private readonly MethodInfo _mi;
            private bool _excludeReturnType;

            internal Sig(MethodInfo mi)
            {
                _mi = mi;
            }

            public Sig WithoutReturnType()
            {
                _excludeReturnType = true;
                return this;
            }

            public static implicit operator string(Sig s)
            {
                return s.ToString();
            }

            public override string ToString()
            {
                var paramTypes = _mi.GetParameters().Select(p => p.ParameterType.FullName);

                var methodSig = String.Format("{0}({1})", _mi.Name, String.Join(",", paramTypes));
                if (!_excludeReturnType)
                {
                    methodSig = _mi.ReturnType.FullName + " " + methodSig;
                }
                return methodSig;
            }
        }
    }
}
