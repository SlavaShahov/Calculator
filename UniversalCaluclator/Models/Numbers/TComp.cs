using System;
using System.Collections.Generic;
using System.Globalization;

namespace UniversalCaluclator.Models.Numbers;

public class TComp : TANumber
{
    private double _re;
    private double _im;

    public double Re           => _re;
    public double Im           => _im;
    public double Modulus      => Math.Sqrt(_re * _re + _im * _im);
    public double ArgumentRad  => Math.Atan2(_im, _re);
    public double ArgumentDeg  => ArgumentRad * 180.0 / Math.PI;

    public TComp(double re = 0, double im = 0) { _re = re; _im = im; }
    public TComp(TComp other) { _re = other._re; _im = other._im; }

    public override string NumberString
    {
        get
        {
            double re = Math.Abs(_re) < 1e-12 ? 0.0 : _re;
            double im = Math.Abs(_im) < 1e-12 ? 0.0 : _im;
            string reStr = re.ToString("G6", CultureInfo.InvariantCulture);
            string sign  = im >= 0 ? "+" : "";
            string imStr = im.ToString("G6", CultureInfo.InvariantCulture);
            return $"{reStr}{sign}{imStr}i";   // всегда a+bi
        }
        set => ParseString(value.Trim());
    }

    private void ParseString(string s)
    {
        if (string.IsNullOrEmpty(s)) 
        { 
            _re = 0; 
            _im = 0; 
            return; 
        }
        
        s = s.Trim();
        int iIdx = s.LastIndexOf('i');
        
        if (iIdx < 0)
        {
            // Нет символа i - это действительное число
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
            {
                _re = val;
                _im = 0;
            }
            return;
        }
        
        // Есть символ i
        string imPart = s[..iIdx].Trim();
        
        // Ищем последний знак + или -
        int sep = -1;
        for (int k = imPart.Length - 1; k > 0; k--)
        {
            if (imPart[k] == '+' || imPart[k] == '-')
            {
                sep = k;
                break;
            }
        }
        
        if (sep >= 0)
        {
            // Действительная часть
            string reStr = imPart[..sep].Trim();
            if (!string.IsNullOrEmpty(reStr))
                double.TryParse(reStr, NumberStyles.Any, CultureInfo.InvariantCulture, out _re);
            else
                _re = 0;
            
            // Мнимая часть
            string imStr = imPart[sep..].Trim();
            ParseImaginaryPart(imStr, out _im);
        }
        else
        {
            // Только мнимая часть
            _re = 0;
            ParseImaginaryPart(imPart, out _im);
        }
    }

    private void ParseImaginaryPart(string imStr, out double im)
    {
        im = 0;
        if (string.IsNullOrEmpty(imStr) || imStr == "+")
            im = 1;
        else if (imStr == "-")
            im = -1;
        else
            double.TryParse(imStr, NumberStyles.Any, CultureInfo.InvariantCulture, out im);
    }

    // ── Специальные операции комплексных чисел ──────────────────────────────

    /// <summary>Возведение в целую степень n (формула Муавра).</summary>
    public TComp Power(int n)
    {
        if (n == 0) return new TComp(1, 0);
        if (n < 0)
        {
            var pos = Power(-n);
            return (TComp)pos.Rev();
        }
        double r     = Math.Pow(Math.Sqrt(_re * _re + _im * _im), n);
        double theta = Math.Atan2(_im, _re) * n;
        return new TComp(r * Math.Cos(theta), r * Math.Sin(theta));
    }

    /// <summary>Все n корней n-й степени.</summary>
    public List<TComp> Roots(int n)
    {
        if (n <= 0) throw new ArgumentException("Степень корня должна быть > 0");
        var list   = new List<TComp>();
        double r   = Math.Pow(Math.Sqrt(_re * _re + _im * _im), 1.0 / n);
        double phi = Math.Atan2(_im, _re);
        for (int k = 0; k < n; k++)
        {
            double theta = (phi + 2 * Math.PI * k) / n;
            list.Add(new TComp(r * Math.Cos(theta), r * Math.Sin(theta)));
        }
        return list;
    }

    // ── Арифметика ──────────────────────────────────────────────────────────

    public override TANumber Add(TANumber b)
    { var c = Cast(b); return new TComp(_re + c._re, _im + c._im); }

    public override TANumber Sub(TANumber b)
    { var c = Cast(b); return new TComp(_re - c._re, _im - c._im); }

    public override TANumber Mul(TANumber b)
    {
        var c = Cast(b);
        return new TComp(_re * c._re - _im * c._im, _re * c._im + _im * c._re);
    }

    public override TANumber Div(TANumber b)
    {
        var c = Cast(b);
        double d = c._re * c._re + c._im * c._im;
        if (d == 0) throw new DivideByZeroException("Деление на ноль");
        return new TComp((_re * c._re + _im * c._im) / d,
                         (_im * c._re - _re * c._im) / d);
    }

    public override TANumber Sqr() => new TComp(_re * _re - _im * _im, 2 * _re * _im);

    public override TANumber Rev()
    {
        double d = _re * _re + _im * _im;
        if (d == 0) throw new DivideByZeroException("Деление на ноль");
        return new TComp(_re / d, -_im / d);
    }

    public override bool EqZero() => Math.Abs(_re) < 1e-12 && Math.Abs(_im) < 1e-12;
    public override bool EqNumber(TANumber b) 
    { 
        var c = Cast(b);
        return Math.Abs(_re - c._re) < 1e-12 && Math.Abs(_im - c._im) < 1e-12;
    }
    public override TANumber Copy() => new TComp(this);

    private TComp Cast(TANumber b)
    {
        if (b is TComp c) return c;
        throw new InvalidOperationException("Несовместимые типы");
    }
}