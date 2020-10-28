using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace FraudDetection
{
    public class Parameter
    {
        public string _fieldId;
        public ParameterValue _value;

    }

    [StructLayout(LayoutKind.Explicit)]
    public struct ParameterValueV
    {
        [FieldOffset(0)] public int i;
        [FieldOffset(0)] public decimal d;
        [FieldOffset(0)] public bool b;
        [FieldOffset(0)] public float f;
        [FieldOffset(0)] public DateTime dt;
    }

    public struct ParameterValue
    {
        public ParameterValueV v;
        public string s;

        public ParameterValue(int o) : this()
        {
            this.v.i = o;
        }
        public ParameterValue(decimal o) : this()
        {
            this.v.d = o;
        }
        public ParameterValue(bool o) : this()
        {
            this.v.b = o;
        }
        public ParameterValue(string o) : this()
        {
            this.s = o;
        }
    }
}
