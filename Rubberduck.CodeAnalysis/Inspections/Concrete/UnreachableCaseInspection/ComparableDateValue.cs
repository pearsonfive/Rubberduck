﻿using Antlr4.Runtime;
using Rubberduck.Parsing.PreProcessing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Rubberduck.Inspections.Concrete.UnreachableCaseInspection
{
    public class ComparableDateValue : IValue, IComparable<ComparableDateValue>
    {
        private readonly DateValue _dateValue;
        private readonly int _hashCode;

        public ComparableDateValue(DateValue dateValue)
        {
            _dateValue = dateValue;
            _hashCode = dateValue.AsDecimal.GetHashCode();
        }

        public Parsing.PreProcessing.ValueType ValueType => _dateValue.ValueType;

        public bool AsBool => _dateValue.AsBool;

        public byte AsByte => _dateValue.AsByte;

        public decimal AsDecimal => _dateValue.AsDecimal;

        public DateTime AsDate => _dateValue.AsDate;

        public string AsString => _dateValue.AsString;

        public IEnumerable<IToken> AsTokens => _dateValue.AsTokens;

        public int CompareTo(ComparableDateValue dateValue)
            => _dateValue.AsDecimal.CompareTo(dateValue._dateValue.AsDecimal);

        public override int GetHashCode() => _hashCode;

        public override bool Equals(object obj)
        {
            if (obj is ComparableDateValue decorator)
            {
                return decorator.CompareTo(this) == 0;
            }

            if (obj is DateValue dateValue)
            {
                return dateValue.AsDecimal == _dateValue.AsDecimal;
            }

            return false;
        }

        public override string ToString()
        {
            return _dateValue.ToString();
        }

        public string AsDateLiteral()
        {
            var asString = AsDate.ToString(CultureInfo.InvariantCulture);
            var prePostPend = "#";
            var result = asString.StartsWith(prePostPend) ? asString : $"{prePostPend}{asString}";
            result = result.EndsWith(prePostPend) ? result : $"{result}{prePostPend}";
            return result;
        }
    }
}
