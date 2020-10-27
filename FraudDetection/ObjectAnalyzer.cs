using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FraudDetection
{
    public class ObjectAnalyzer
    {
        Dictionary<string, PInfo> _map = new Dictionary<string, PInfo>(StringComparer.InvariantCultureIgnoreCase);
        public void Analyze(Type t)
        {
            foreach (var p in t.GetProperties())
            {
                _map.Add(p.Name, new PInfo((PropertyInfo)p));
            }
        }
        public void Analyze<T>()
        {
            Type t = typeof(T);
            Analyze(t);
        }

        public void SetValue<T>(T o, string propName, string value)
        {
            if (_map.TryGetValue(propName, out var pinfo))
            {
                pinfo.SetValue(o, value);
            }
        }
    }

    public class PInfo
    {
        PropertyInfo _info;
        public PInfo(PropertyInfo info)
        {
            _info = info;
        }
        public void SetValue<T>(T o, string value)
        {
            switch (_info.PropertyType.Name)
            {
                case "DateTime":
                    _info.SetMethod.Invoke(o, new object[] { DateTime.Parse(value) });
                    break;
                case "Decimal":
                    _info.SetMethod.Invoke(o, new object[] { Decimal.Parse(value) });
                    break;
                case "Int32":
                    _info.SetMethod.Invoke(o, new object[] { Int32.Parse(value) });
                    break;
                case "Boolean":
                    _info.SetMethod.Invoke(o, new object[] { Boolean.Parse(value) });
                    break;
                case "String":
                    _info.SetMethod.Invoke(o, new object[] { value });
                    break;
                default:
                    _info.SetMethod.Invoke(o, new object[] { value });
                    break;
            }
        }
    }
}
