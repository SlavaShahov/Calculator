using System;
using System.Globalization;
using System.Text;

namespace UniversalCaluclator.Models.Numbers;

public class TPNumber : TANumber
{
    private double _value;
    private int _base;
    private int _precision;

    public int Base
    {
        get => _base;
        set => _base = value is >= 2 and <= 16 ? value : 10;
    }

    public int Precision
    {
        get => _precision;
        set => _precision = value >= 0 ? value : 0;
    }

    public double Value => _value;

    public TPNumber(double value = 0, int numBase = 10, int precision = 8)
    {
        _value = value;
        Base = numBase;
        _precision = precision;
    }

    public TPNumber(TPNumber other)
    {
        _value = other._value;
        _base = other._base;
        _precision = other._precision;
    }

    public override string NumberString
    {
        get => DoubleToBaseString(_value, _base, _precision);
        set => _value = BaseStringToDouble(value, _base);
    }

    private static string DoubleToBaseString(double v, int b, int prec)
    {
        if (b == 10)
            return v.ToString(prec > 0 ? $"G{prec + 2}" : "G", CultureInfo.InvariantCulture);

        bool neg = v < 0;
        v = Math.Abs(v);
        long intPart = (long)Math.Truncate(v);
        double fracPart = v - intPart;
        string intStr = IntToBase(intPart, b);

        if (prec == 0)
            return (neg ? "-" : "") + intStr;

        var sb = new StringBuilder();
        for (int i = 0; i < prec; i++)
        {
            fracPart *= b;
            int digit = (int)fracPart;
            sb.Append(DigitChar(digit));
            fracPart -= digit;
        }
        return (neg ? "-" : "") + intStr + "." + sb;
    }

    private static string IntToBase(long v, int b)
    {
        if (v == 0) return "0";
        var sb = new StringBuilder();
        while (v > 0)
        {
            sb.Insert(0, DigitChar((int)(v % b)));
            v /= b;
        }
        return sb.ToString();
    }

    private static char DigitChar(int d) => d < 10 ? (char)('0' + d) : (char)('A' + d - 10);

    private static double BaseStringToDouble(string s, int b)
    {
        if (string.IsNullOrWhiteSpace(s) || s == "0") return 0;
        s = s.Trim().ToUpper();
        bool neg = s.StartsWith('-');
        if (neg) s = s[1..];

        if (b == 10)
            return (neg ? -1 : 1) * double.Parse(s, CultureInfo.InvariantCulture);

        int dot = s.IndexOf('.');
        string intPart = dot < 0 ? s : s[..dot];
        string fracStr = dot < 0 ? "" : s[(dot + 1)..];

        double result = 0;
        foreach (char c in intPart)
            result = result * b + CharToDigit(c);

        double factor = 1.0 / b;
        foreach (char c in fracStr)
        {
            result += CharToDigit(c) * factor;
            factor /= b;
        }
        return neg ? -result : result;
    }

    private static int CharToDigit(char c) => c >= 'A' ? c - 'A' + 10 : c - '0';

    public override TANumber Add(TANumber b) => new TPNumber(_value + Cast(b)._value, _base, _precision);
    public override TANumber Sub(TANumber b) => new TPNumber(_value - Cast(b)._value, _base, _precision);
    public override TANumber Mul(TANumber b) => new TPNumber(_value * Cast(b)._value, _base, _precision);
    public override TANumber Div(TANumber b)
    {
        var bv = Cast(b)._value;
        if (bv == 0) throw new DivideByZeroException("Деление на ноль");
        return new TPNumber(_value / bv, _base, _precision);
    }
    public override TANumber Sqr() => new TPNumber(_value * _value, _base, _precision);
    public override TANumber Rev()
    {
        if (_value == 0) throw new DivideByZeroException("Деление на ноль");
        return new TPNumber(1.0 / _value, _base, _precision);
    }
    public override bool EqZero() => _value == 0;
    public override bool EqNumber(TANumber b) => Math.Abs(_value - Cast(b)._value) < 1e-12;
    public override TANumber Copy() => new TPNumber(this);

    private TPNumber Cast(TANumber b)
    {
        if (b is TPNumber p) return p;
        throw new InvalidOperationException("Несовместимые типы");
    }
}
