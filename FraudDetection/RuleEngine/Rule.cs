using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FraudDetection.RuleEngine
{
    public class Rule
    {
        public DateTime _expirationTime { get; private set; }
        public List<Parameter> _parameters;
        int triggeredAmount = 0;

        public bool IsExpired(DateTime dtNow)
        {
            return _expirationTime < dtNow;
        }

        public bool IsMatching(Transaction t)
        {
            bool isMatching = true;
            foreach (var p in _parameters)
                isMatching &= TransactionHelper.IsMatching(p._fieldId, p._value, t);

            return isMatching;
        }

        public void ActivateRule(DateTime dtNow)
        {
            int banIndex = triggeredAmount < FraudDetectionDefaults.banInMinutes.Length ? triggeredAmount : FraudDetectionDefaults.banInMinutes.Length - 1;
            _expirationTime = dtNow.Add(FraudDetectionDefaults.banInMinutes[banIndex]);
            triggeredAmount++;
        }

        public void AddParameter(FIELDID fieldId, ParameterValue value)
        {
            _parameters ??= new List<Parameter>();
            _parameters.Add(new Parameter()
            {
                _fieldId = fieldId,
                _value = value,
            });
        }

        public void Normalize()
        {
            _parameters.OrderBy(x => x._fieldId);
        }

        public static bool Compare(Rule r1, Rule r2)
        {
            if ((r1 == null) && (r2 == null))
                return true;
            if ((r1 == null) || (r2 == null))
                return false;

            if ((r1._parameters == null) && (r2._parameters == null))
                return true;
            if ((r1._parameters == null) || (r2._parameters == null))
                return false;

            if (r1._parameters.Count != r2._parameters.Count)
                return false;
            for (int i = 0; i < r1._parameters.Count; i++)
            {
                if (!Parameter.Compare(r1._parameters[i], r2._parameters[i]))
                    return false;
            }
            return true;
        }
    }


}
