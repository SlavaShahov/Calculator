using System;

namespace UniversalCaluclator.Models.Numbers;

public class TFrac : TANumber
{
    private long _num;
    private long _den;

    public long Numerator => _num;
    public long Denominator => _den;

    public TFrac(long num = 0, long den = 1)
    {
        if (den == 0) throw new DivideByZeroException("Знаменатель не может быть нулём");
        _num = num;
        _den = den;
        Reduce();
    }

    public TFrac(TFrac other) { _num = other._num; _den = other._den; }

    public override string NumberString
    {
        get => _den == 1 ? _num.ToString() : $"{_num}/{_den}";
        set
        {
            var s = value.Trim();
            int slash = s.IndexOf('/');
            if (slash < 0)
            {
                _num = long.Parse(s);
                _den = 1;
            }
            else
            {
                _num = long.Parse(s[..slash]);
                _den = long.Parse(s[(slash + 1)..]);
            }
            Reduce();
        }
    }

    public override TANumber Add(TANumber b)
    {
        var f = Cast(b);
        return new TFrac(_num * f._den + f._num * _den, _den * f._den);
    }
    public override TANumber Sub(TANumber b)
    {
        var f = Cast(b);
        return new TFrac(_num * f._den - f._num * _den, _den * f._den);
    }
    public override TANumber Mul(TANumber b)
    {
        var f = Cast(b);
        return new TFrac(_num * f._num, _den * f._den);
    }
    public override TANumber Div(TANumber b)
    {
        var f = Cast(b);
        if (f._num == 0) throw new DivideByZeroException("Деление на ноль");
        return new TFrac(_num * f._den, _den * f._num);
    }
    public override TANumber Sqr() => new TFrac(_num * _num, _den * _den);
    public override TANumber Rev()
    {
        if (_num == 0) throw new DivideByZeroException("Деление на ноль");
        return new TFrac(_den, _num);
    }
    public override bool EqZero() => _num == 0;
    public override bool EqNumber(TANumber b) => Cast(b)._num == _num && Cast(b)._den == _den;
    public override TANumber Copy() => new TFrac(this);

    private void Reduce()
    {
        if (_den < 0) { _num = -_num; _den = -_den; }
        long g = Gcd(Math.Abs(_num), _den);
        if (g > 1) { _num /= g; _den /= g; }
    }

    private static long Gcd(long a, long b) => b == 0 ? a : Gcd(b, a % b);

    private TFrac Cast(TANumber b)
    {
        if (b is TFrac f) return f;
        throw new InvalidOperationException("Несовместимые типы");
    }
}
