using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace FraudDetection
{
    public enum FIELDID { IP = 1, sourceId = 2, amount = 3, declined = 4 }

    public class Transaction
    {
        public DateTime transactionTime { get; set; }
        public decimal amount { get; set; }
        public int sourceId { get; set; }
        public bool declined { get; set; }
        public string ip { get; set; }
    }

    public class TransactionHelper
    {
        public static bool populateTransaction(string name, string value, Transaction tr)
        {
            switch (name)
            {
                case "amount":
                    tr.amount = Decimal.Parse(value);
                    return true;
                case "sourceid":
                    tr.sourceId = Int32.Parse(value);
                    return true;
                case "declined":
                    tr.declined = Boolean.Parse(value);
                    return true;
                case "ip":
                    tr.ip = value;
                    return true;
                default:
                    return false;
            }
        }
        public static Parameter GetValue(string name, string value)
        {
            var parm = new Parameter();
            switch (name)
            {
                case "amount":
                    parm._fieldId = FIELDID.amount;
                    parm._value.v.d = Decimal.Parse(value);
                    break;
                case "sourceid":
                    parm._fieldId = FIELDID.sourceId;
                    parm._value.v.i = Int32.Parse(value);
                    break;
                case "declined":
                    parm._fieldId = FIELDID.amount;
                    parm._value.v.b = Boolean.Parse(value);
                    break;
                case "ip":
                    parm._fieldId = FIELDID.IP;
                    parm._value.s = value;
                    break;
                default:
                    throw new Exception($"Unknown field {name}");
            }
            return parm;
        }

        public static bool IsMatching(FIELDID fieldId, ParameterValue o, Transaction tr)
        {
            switch (fieldId)
            {
                case FIELDID.IP:
                    if (String.Compare(tr.ip, o.s, true) == 0)
                        return true;
                    return false;
                case FIELDID.sourceId:
                    if (tr.sourceId == o.v.i)
                        return true;
                    return false;
                case FIELDID.amount:
                    if (tr.amount >= o.v.d)
                        return true;
                    return false;
                case FIELDID.declined:
                    if (tr.declined == o.v.b)
                        return true;
                    return false;
                default:
                    throw new Exception($"Unknown fieldId {fieldId}");
            }
        }
        public static bool Compare(FIELDID fieldId, ParameterValue o1, ParameterValue o2)
        {
            switch (fieldId)
            {
                case FIELDID.IP:
                    if (String.Compare(o1.s, o2.s, true) == 0)
                        return true;
                    return false;
                case FIELDID.sourceId:
                    if (o1.v.i == o2.v.i)
                        return true;
                    return false;
                case FIELDID.amount:
                    if (o1.v.d == o2.v.d)
                        return true;
                    return false;
                case FIELDID.declined:
                    if (o1.v.b == o2.v.b)
                        return true;
                    return false;
                default:
                    throw new Exception($"Unknown fieldId {fieldId}");
            }
        }

        public static bool CompareTransactions(FIELDID[] fields, Transaction t1, Transaction t2)
        {
            if ((fields == null) || (fields.Length == 0))
                return true; //all transactions are equial, we have nothing to compare
            foreach (var f in fields)
            {
                switch (f)
                {
                    case FIELDID.IP:
                        if (t1.ip != t2.ip)
                            return false;
                        break;
                    case FIELDID.sourceId:
                        if (t1.sourceId != t2.sourceId)
                            return false;
                        break;
                    case FIELDID.amount:
                        if (t1.amount != t2.amount)
                            return false;
                        break;
                    case FIELDID.declined:
                        if (t1.declined != t2.declined)
                            return false;
                        break;
                    default:
                        throw new Exception($"Unknown fieldId {f}");
                }
            }

            return true;
        }

    }

  
}
