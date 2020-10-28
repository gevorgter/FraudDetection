using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace FraudDetection
{
    public static class TransactionHelper
    {
        static Dictionary<string, PInfo<Transaction>> _map = new Dictionary<string, PInfo<Transaction>>(StringComparer.InvariantCultureIgnoreCase);
        static public void Init()
        {
            Type t = typeof(Transaction);
            foreach (var p in t.GetProperties())
            {
                _map.Add(p.Name, new PInfo<Transaction>((PropertyInfo)p));
            }
        }

        static public void SetValue(Transaction o, string propName, string value)
        {
            if (_map.TryGetValue(propName, out var pinfo))
            {
                pinfo.SetValue(o, value);
            }
        }
        static public bool Compare(IEnumerable<string> fields, Transaction o1, Transaction o2)
        {
            if (fields == null)
                return true;
            foreach (var s in fields)
            {
                if (_map.TryGetValue(s, out var pinfo))
                {
                    if (!pinfo.Compare(o1, o2))
                        return false;
                }
            }
            return true;
        }
        static public bool Compare(IEnumerable<Parameter> fields, Transaction o1)
        {
            if (fields == null)
                return true;
            foreach (var f in fields)
            {
                if (_map.TryGetValue(f._fieldId, out var pinfo))
                {
                    if (!pinfo.Compare(o1, f))
                        return false;
                }
            }
            return true;
        }
        static public bool Compare(Parameter p1, Parameter p2)
        {
            if (String.Compare(p1._fieldId, p2._fieldId, true) != 0)
                return false;
            if (_map.TryGetValue(p1._fieldId, out var pinfo))
            {
                if (!pinfo.Compare(p1,p2))
                    return false;
            }
            return true;
        }
    }

    public abstract class Setter<T>
    {
        public abstract void SetValue(T o, string value);
        public abstract bool Compare(T o1, T o2);
        public abstract bool Compare(T o1, Parameter p1);
        public abstract bool Compare(Parameter p1, Parameter p2);


    }
    public class SetterDateTime<T>: Setter<T>
    {
        public Action<T, DateTime> _setter;
        public Func<T, DateTime> _getter;
        public SetterDateTime(MethodInfo setter, MethodInfo getter)
        {
            _setter = (Action<T, DateTime>)setter.CreateDelegate(typeof(Action<T, DateTime>));
            _getter = (Func<T, DateTime>)getter.CreateDelegate(typeof(Func<T, DateTime>));
        }

        public override void SetValue(T o, string value)
        {
            _setter(o, DateTime.Parse(value));
        }
        public override bool Compare(T o1, T o2 )
        {
            var v1 = _getter(o1);
            var v2 = _getter(o2);
            return v1 == v2;
        }
        public override bool Compare(T o1, Parameter p1)
        {
            var v1 = _getter(o1);
            return v1 == p1._value.v.dt;
        }
        public override bool Compare(Parameter p1, Parameter p2)
        {
            return p1._value.v.dt == p2._value.v.dt;
        }
    }

    public class SetterDecimal<T> : Setter<T>
    {
        public Action<T, Decimal> _setter;
        public Func<T, Decimal> _getter;
        public SetterDecimal(MethodInfo setter, MethodInfo getter)
        {
            _setter = (Action<T, Decimal>)setter.CreateDelegate(typeof(Action<T, Decimal>));
            _getter = (Func<T, Decimal>)getter.CreateDelegate(typeof(Func<T, Decimal>));
        }

        public override void SetValue(T o, string value)
        {
            _setter(o, Decimal.Parse(value));
        }
        public override bool Compare(T o1, T o2)
        {
            var v1 = _getter(o1);
            var v2 = _getter(o2);
            return v1 == v2;
        }
        public override bool Compare(T o1, Parameter p1)
        {
            var v1 = _getter(o1);
            return v1 == p1._value.v.d;
        }
        public override bool Compare(Parameter p1, Parameter p2)
        {
            return p1._value.v.d == p2._value.v.d;
        }
    }

    public class SetterBoolean<T> : Setter<T>
    {
        public Action<T, Boolean> _setter;
        public Func<T, Boolean> _getter;
        public SetterBoolean(MethodInfo setter, MethodInfo getter)
        {
            _setter = (Action<T, Boolean>)setter.CreateDelegate(typeof(Action<T, Boolean>));
            _getter = (Func<T, Boolean>)getter.CreateDelegate(typeof(Func<T, Boolean>));
        }

        public override void SetValue(T o, string value)
        {
            _setter(o, Boolean.Parse(value));
        }
        public override bool Compare(T o1, T o2)
        {
            var v1 = _getter(o1);
            var v2 = _getter(o2);
            return v1 == v2;
        }
        public override bool Compare(T o1, Parameter p1)
        {
            var v1 = _getter(o1);
            return v1 == p1._value.v.b;
        }
        public override bool Compare(Parameter p1, Parameter p2)
        {
            return p1._value.v.b == p2._value.v.b;
        }
    }

    public class SetterInt32<T> : Setter<T>
    {
        public Action<T, Int32> _setter;
        public Func<T, Int32> _getter;
        public SetterInt32(MethodInfo setter, MethodInfo getter)
        {
            _setter = (Action<T, Int32>)setter.CreateDelegate(typeof(Action<T, Int32>));
            _getter = (Func<T, Int32>)getter.CreateDelegate(typeof(Func<T, Int32>));
        }

        public override void SetValue(T o, string value)
        {
            _setter(o, Int32.Parse(value));
        }
        public override bool Compare(T o1, T o2)
        {
            var v1 = _getter(o1);
            var v2 = _getter(o2);
            return v1 == v2;
        }
        public override bool Compare(T o1, Parameter p1)
        {
            var v1 = _getter(o1);
            return v1 == p1._value.v.i;
        }
        public override bool Compare(Parameter p1, Parameter p2)
        {
            return p1._value.v.i == p2._value.v.i;
        }
    }

    public class SetterString<T> : Setter<T>
    {
        public Action<T, String> _setter;
        public Func<T, String> _getter;
        public SetterString(MethodInfo setter, MethodInfo getter)
        {
            _setter = (Action<T, String>)setter.CreateDelegate(typeof(Action<T, String>));
            _getter = (Func<T, String>)getter.CreateDelegate(typeof(Func<T, String>));
        }

        public override void SetValue(T o, string value)
        {
            _setter(o, value);
        }
        public override bool Compare(T o1, T o2)
        {
            var v1 = _getter(o1);
            var v2 = _getter(o2);
            return String.Compare(v1, v2, true) == 0;
        }
        public override bool Compare(T o1, Parameter p1)
        {
            var v1 = _getter(o1);
            return String.Compare(v1,p1._value.s, true)==0;
        }
        public override bool Compare(Parameter p1, Parameter p2)
        {
            return String.Compare(p1._value.s, p2._value.s, true) == 0;
        }
    }

    public class PInfo<T>
    {
        PropertyInfo _info;
        Setter<T> _setter;

        public PInfo(PropertyInfo info)
        {
            _info = info;
            MethodInfo m = info.SetMethod;
            switch (info.PropertyType.Name)
            {
                case "DateTime":
                    _setter = new SetterDateTime<T>(info.SetMethod, info.GetMethod);
                    break;
                case "Decimal":
                    _setter = new SetterDecimal<T>(info.SetMethod, info.GetMethod);
                    break;
                case "Int32":
                    _setter = new SetterInt32<T>(info.SetMethod, info.GetMethod);
                    break;
                case "Boolean":
                    _setter = new SetterBoolean<T>(info.SetMethod, info.GetMethod);
                    break;
                case "String":
                    _setter = new SetterString<T>(info.SetMethod, info.GetMethod);
                    break;
            }
        }
        public void SetValue(T o, string value)
        {
            _setter.SetValue(o, value);
        }
        public bool Compare(T o1, T o2)
        {
            return _setter.Compare(o1, o2);
        }
        public bool Compare(T o1, Parameter p1)
        {
            return _setter.Compare(o1, p1);
        }
        public bool Compare(Parameter p1, Parameter p2)
        {
            return _setter.Compare(p1, p2);
        }
    }
}
