using System;
using Xunit;
using UniversalCaluclator.Models.Numbers;
using UniversalCaluclator.Models.Editors;
using UniversalCaluclator.Models.Memory;
using UniversalCaluclator.Models.Processor;
using UniversalCaluclator.Models.Controller;

namespace UniversalCalculator.Tests;


// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
//  ТЕСТЫ: TComp — комплексные числа
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
public class TCompTests
{
    // ── Конструктор и свойства ───────────────────────────────────────────────

    [Fact]
    public void Constructor_Default_ReturnsZero()
    {
        var z = new TComp();
        Assert.Equal(0, z.Re);
        Assert.Equal(0, z.Im);
    }

    [Fact]
    public void Constructor_WithValues_StoresCorrectly()
    {
        var z = new TComp(3, 4);
        Assert.Equal(3, z.Re);
        Assert.Equal(4, z.Im);
    }

    [Fact]
    public void Constructor_Copy_CreatesIndependentCopy()
    {
        var a = new TComp(3, 4);
        var b = new TComp(a);
        Assert.Equal(a.Re, b.Re);
        Assert.Equal(a.Im, b.Im);
    }

    // ── Модуль и аргумент ────────────────────────────────────────────────────

    [Fact]
    public void Modulus_ThreePlusFourI_ReturnsFive()
    {
        // |3+4i| = sqrt(9+16) = sqrt(25) = 5
        var z = new TComp(3, 4);
        Assert.Equal(5.0, z.Modulus, precision: 10);
    }

    [Fact]
    public void Modulus_Zero_ReturnsZero()
    {
        var z = new TComp(0, 0);
        Assert.Equal(0.0, z.Modulus, precision: 10);
    }

    [Fact]
    public void Modulus_PureImaginary_ReturnsAbsIm()
    {
        // |0+3i| = 3
        var z = new TComp(0, 3);
        Assert.Equal(3.0, z.Modulus, precision: 10);
    }

    [Fact]
    public void ArgumentDeg_PositiveReal_ReturnsZero()
    {
        // arg(1+0i) = 0°
        var z = new TComp(1, 0);
        Assert.Equal(0.0, z.ArgumentDeg, precision: 10);
    }

    [Fact]
    public void ArgumentDeg_PurePositiveImaginary_Returns90()
    {
        // arg(0+1i) = 90°
        var z = new TComp(0, 1);
        Assert.Equal(90.0, z.ArgumentDeg, precision: 10);
    }

    [Fact]
    public void ArgumentDeg_NegativeReal_Returns180()
    {
        // arg(-1+0i) = 180°
        var z = new TComp(-1, 0);
        Assert.Equal(180.0, z.ArgumentDeg, precision: 10);
    }

    [Fact]
    public void ArgumentDeg_PureNegativeImaginary_ReturnsMinus90()
    {
        // arg(0-1i) = -90°
        var z = new TComp(0, -1);
        Assert.Equal(-90.0, z.ArgumentDeg, precision: 10);
    }

    [Fact]
    public void ArgumentRad_PurePositiveImaginary_ReturnsHalfPi()
    {
        // arg(0+1i) = π/2
        var z = new TComp(0, 1);
        Assert.Equal(Math.PI / 2, z.ArgumentRad, precision: 10);
    }

    // ── Сложение ─────────────────────────────────────────────────────────────

    [Fact]
    public void Add_TwoComplex_ReturnsCorrectSum()
    {
        // (3+4i) + (1+2i) = 4+6i
        var a = new TComp(3, 4);
        var b = new TComp(1, 2);
        var r = (TComp)a.Add(b);
        Assert.Equal(4, r.Re, precision: 10);
        Assert.Equal(6, r.Im, precision: 10);
    }

    [Fact]
    public void Add_OppositeNumbers_ReturnsZero()
    {
        // (3+4i) + (-3-4i) = 0
        var a = new TComp(3, 4);
        var b = new TComp(-3, -4);
        var r = (TComp)a.Add(b);
        Assert.True(r.EqZero());
    }

    [Fact]
    public void Add_WithZero_ReturnsSameNumber()
    {
        // (3+4i) + 0 = 3+4i
        var a = new TComp(3, 4);
        var b = new TComp(0, 0);
        var r = (TComp)a.Add(b);
        Assert.Equal(3, r.Re, precision: 10);
        Assert.Equal(4, r.Im, precision: 10);
    }

    // ── Вычитание ────────────────────────────────────────────────────────────

    [Fact]
    public void Sub_TwoComplex_ReturnsCorrectDifference()
    {
        // (5+3i) - (2+1i) = 3+2i
        var a = new TComp(5, 3);
        var b = new TComp(2, 1);
        var r = (TComp)a.Sub(b);
        Assert.Equal(3, r.Re, precision: 10);
        Assert.Equal(2, r.Im, precision: 10);
    }

    [Fact]
    public void Sub_SameNumbers_ReturnsZero()
    {
        var a = new TComp(3, 4);
        var r = (TComp)a.Sub(a);
        Assert.True(r.EqZero());
    }

    // ── Умножение ────────────────────────────────────────────────────────────

    [Fact]
    public void Mul_TwoComplex_ReturnsCorrectProduct()
    {
        // (2+3i) * (1+4i) = 2 + 8i + 3i + 12i² = 2+11i-12 = -10+11i
        var a = new TComp(2, 3);
        var b = new TComp(1, 4);
        var r = (TComp)a.Mul(b);
        Assert.Equal(-10, r.Re, precision: 10);
        Assert.Equal(11,  r.Im, precision: 10);
    }

    [Fact]
    public void Mul_ByOne_ReturnsSameNumber()
    {
        // (3+4i) * 1 = 3+4i
        var a = new TComp(3, 4);
        var b = new TComp(1, 0);
        var r = (TComp)a.Mul(b);
        Assert.Equal(3, r.Re, precision: 10);
        Assert.Equal(4, r.Im, precision: 10);
    }

    [Fact]
    public void Mul_ByI_RotatesBy90Degrees()
    {
        // (1+0i) * i = 0+1i (поворот на 90°)
        var a = new TComp(1, 0);
        var i = new TComp(0, 1);
        var r = (TComp)a.Mul(i);
        Assert.Equal(0, r.Re, precision: 10);
        Assert.Equal(1, r.Im, precision: 10);
    }

    [Fact]
    public void Mul_IByI_ReturnsMinusOne()
    {
        // i * i = i² = -1
        var i = new TComp(0, 1);
        var r = (TComp)i.Mul(i);
        Assert.Equal(-1, r.Re, precision: 10);
        Assert.Equal(0,  r.Im, precision: 10);
    }

    // ── Деление ──────────────────────────────────────────────────────────────

    [Fact]
    public void Div_TwoComplex_ReturnsCorrectQuotient()
    {
        // (4+2i) / (1+1i) = (4+2i)(1-i) / 2 = (4-4i+2i-2i²) / 2 = (6-2i) / 2 = 3-1i
        var a = new TComp(4, 2);
        var b = new TComp(1, 1);
        var r = (TComp)a.Div(b);
        Assert.Equal(3,  r.Re, precision: 10);
        Assert.Equal(-1, r.Im, precision: 10);
    }

    [Fact]
    public void Div_ByZero_ThrowsDivideByZeroException()
    {
        var a = new TComp(1, 0);
        var b = new TComp(0, 0);
        Assert.Throws<DivideByZeroException>(() => a.Div(b));
    }

    [Fact]
    public void Div_ByOne_ReturnsSameNumber()
    {
        // (3+4i) / 1 = 3+4i
        var a = new TComp(3, 4);
        var b = new TComp(1, 0);
        var r = (TComp)a.Div(b);
        Assert.Equal(3, r.Re, precision: 10);
        Assert.Equal(4, r.Im, precision: 10);
    }

    // ── Квадрат ──────────────────────────────────────────────────────────────

    [Fact]
    public void Sqr_ThreePlusFourI_ReturnsCorrect()
    {
        // (3+4i)² = 9 + 24i + 16i² = 9-16+24i = -7+24i
        var z = new TComp(3, 4);
        var r = (TComp)z.Sqr();
        Assert.Equal(-7, r.Re, precision: 10);
        Assert.Equal(24, r.Im, precision: 10);
    }

    [Fact]
    public void Sqr_ImaginaryUnit_ReturnsMinusOne()
    {
        // i² = -1
        var i = new TComp(0, 1);
        var r = (TComp)i.Sqr();
        Assert.Equal(-1, r.Re, precision: 10);
        Assert.Equal(0,  r.Im, precision: 10);
    }

    [Fact]
    public void Sqr_Zero_ReturnsZero()
    {
        var z = new TComp(0, 0);
        var r = (TComp)z.Sqr();
        Assert.True(r.EqZero());
    }

    // ── Обратное число ───────────────────────────────────────────────────────

    [Fact]
    public void Rev_ImaginaryUnit_ReturnsMinusI()
    {
        // 1/i = -i
        var i = new TComp(0, 1);
        var r = (TComp)i.Rev();
        Assert.Equal(0,  r.Re, precision: 10);
        Assert.Equal(-1, r.Im, precision: 10);
    }

    [Fact]
    public void Rev_One_ReturnsOne()
    {
        // 1/1 = 1
        var z = new TComp(1, 0);
        var r = (TComp)z.Rev();
        Assert.Equal(1, r.Re, precision: 10);
        Assert.Equal(0, r.Im, precision: 10);
    }

    [Fact]
    public void Rev_Zero_ThrowsDivideByZeroException()
    {
        var z = new TComp(0, 0);
        Assert.Throws<DivideByZeroException>(() => z.Rev());
    }

    [Fact]
    public void Rev_ThreePlusFourI_ReturnsCorrect()
    {
        // 1/(3+4i) = (3-4i)/25 = 0.12 - 0.16i
        var z = new TComp(3, 4);
        var r = (TComp)z.Rev();
        Assert.Equal(3.0 / 25, r.Re, precision: 10);
        Assert.Equal(-4.0 / 25, r.Im, precision: 10);
    }

    // ── Возведение в степень ─────────────────────────────────────────────────

    [Fact]
    public void Power_Zero_ReturnsOne()
    {
        // z^0 = 1 для любого z ≠ 0
        var z = new TComp(3, 4);
        var r = z.Power(0);
        Assert.Equal(1, r.Re, precision: 10);
        Assert.Equal(0, r.Im, precision: 10);
    }

    [Fact]
    public void Power_One_ReturnsSameNumber()
    {
        // z^1 = z
        var z = new TComp(3, 4);
        var r = z.Power(1);
        Assert.Equal(3, r.Re, precision: 6);
        Assert.Equal(4, r.Im, precision: 6);
    }

    [Fact]
    public void Power_ISquared_ReturnsMinusOne()
    {
        // i^2 = -1
        var i = new TComp(0, 1);
        var r = i.Power(2);
        Assert.Equal(-1, r.Re, precision: 10);
        Assert.Equal(0,  r.Im, precision: 10);
    }

    [Fact]
    public void Power_ICubed_ReturnsMinusI()
    {
        // i^3 = -i
        var i = new TComp(0, 1);
        var r = i.Power(3);
        Assert.Equal(0,  r.Re, precision: 10);
        Assert.Equal(-1, r.Im, precision: 10);
    }

    [Fact]
    public void Power_Negative_ReturnsInverse()
    {
        // (2+0i)^(-1) = 0.5
        var z = new TComp(2, 0);
        var r = z.Power(-1);
        Assert.Equal(0.5, r.Re, precision: 10);
        Assert.Equal(0,   r.Im, precision: 10);
    }

    [Fact]
    public void Power_OnePlusI_Cubed_ReturnsCorrect()
    {
        // (1+i)^3 = -2+2i
        var z = new TComp(1, 1);
        var r = z.Power(3);
        Assert.Equal(-2, r.Re, precision: 6);
        Assert.Equal(2,  r.Im, precision: 6);
    }

    // ── Корни ────────────────────────────────────────────────────────────────

    [Fact]
    public void Roots_MinusOne_SquareRoots_ReturnsPlusAndMinusI()
    {
        // √(-1) = ±i
        var z = new TComp(-1, 0);
        var roots = z.Roots(2);

        Assert.Equal(2, roots.Count);
        // Первый: +i
        Assert.Equal(0,  roots[0].Re, precision: 6);
        Assert.Equal(1,  roots[0].Im, precision: 6);
        // Второй: -i
        Assert.Equal(0,  roots[1].Re, precision: 6);
        Assert.Equal(-1, roots[1].Im, precision: 6);
    }

    [Fact]
    public void Roots_One_FourthRoots_ReturnsFourValues()
    {
        // Четыре корня из 1: 1, i, -1, -i
        var z = new TComp(1, 0);
        var roots = z.Roots(4);

        Assert.Equal(4, roots.Count);
        Assert.Equal(1,  roots[0].Re, precision: 6);
        Assert.Equal(0,  roots[0].Im, precision: 6);
        Assert.Equal(0,  roots[1].Re, precision: 6);
        Assert.Equal(1,  roots[1].Im, precision: 6);
        Assert.Equal(-1, roots[2].Re, precision: 6);
        Assert.Equal(0,  roots[2].Im, precision: 6);
        Assert.Equal(0,  roots[3].Re, precision: 6);
        Assert.Equal(-1, roots[3].Im, precision: 6);
    }

    [Fact]
    public void Roots_InvalidDegree_ThrowsArgumentException()
    {
        var z = new TComp(1, 0);
        Assert.Throws<ArgumentException>(() => z.Roots(0));
    }

    [Fact]
    public void Roots_CountAlwaysEqualsN()
    {
        var z = new TComp(2, 3);
        for (int n = 1; n <= 5; n++)
            Assert.Equal(n, z.Roots(n).Count);
    }

    // ── EqZero / EqNumber / Copy ─────────────────────────────────────────────

    [Fact]
    public void EqZero_ZeroNumber_ReturnsTrue()
    {
        Assert.True(new TComp(0, 0).EqZero());
    }

    [Fact]
    public void EqZero_NonZeroReal_ReturnsFalse()
    {
        Assert.False(new TComp(1, 0).EqZero());
    }

    [Fact]
    public void EqZero_TinyValue_ReturnsTrue()
    {
        // Значения меньше 1e-12 считаются нулём
        Assert.True(new TComp(1e-13, 1e-13).EqZero());
    }

    [Fact]
    public void EqNumber_EqualNumbers_ReturnsTrue()
    {
        var a = new TComp(3, 4);
        var b = new TComp(3, 4);
        Assert.True(a.EqNumber(b));
    }

    [Fact]
    public void EqNumber_DifferentNumbers_ReturnsFalse()
    {
        var a = new TComp(3, 4);
        var b = new TComp(3, 5);
        Assert.False(a.EqNumber(b));
    }

    [Fact]
    public void Copy_CreatesIndependentInstance()
    {
        var a = new TComp(3, 4);
        var b = (TComp)a.Copy();
        Assert.Equal(a.Re, b.Re);
        Assert.Equal(a.Im, b.Im);
        Assert.False(ReferenceEquals(a, b)); // разные объекты
    }

    // ── Парсинг строк (NumberString) ─────────────────────────────────────────

    [Theory]
    [InlineData("3+4i",  3,  4)]
    [InlineData("3-4i",  3, -4)]
    [InlineData("-2i",   0, -2)]
    [InlineData("5",     5,  0)]
    [InlineData("+i",    0,  1)]
    [InlineData("-i",    0, -1)]
    [InlineData("0+0i",  0,  0)]
    [InlineData("",      0,  0)]
    [InlineData("-3+0i", -3,  0)]
    public void NumberString_SetAndGet_ParsesCorrectly(string input, double re, double im)
    {
        var z = new TComp();
        z.NumberString = input;
        Assert.Equal(re, z.Re, precision: 6);
        Assert.Equal(im, z.Im, precision: 6);
    }

    [Fact]
    public void NumberString_Get_FormatsCorrectly()
    {
        // GET должен вернуть "3+4i"
        var z = new TComp(3, 4);
        Assert.Contains("3", z.NumberString);
        Assert.Contains("4", z.NumberString);
        Assert.Contains("i", z.NumberString);
    }
}

// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
//  ТЕСТЫ: PEditor — редактор P-чисел
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
public class PEditorTests
{
    // ── Начальное состояние ──────────────────────────────────────────────────

    [Fact]
    public void Initial_NumberIsZero()
    {
        var e = new PEditor();
        Assert.Equal("0", e.Number);
    }

    [Fact]
    public void IsZero_OnInit_ReturnsTrue()
    {
        Assert.True(new PEditor().IsZero());
    }

    // ── AddDigit ─────────────────────────────────────────────────────────────

    [Fact]
    public void AddDigit_SingleDigit_ReplacesZero()
    {
        var e = new PEditor();
        e.AddDigit(5);
        Assert.Equal("5", e.Number);
    }

    [Fact]
    public void AddDigit_MultipleDigits_BuildsNumber()
    {
        var e = new PEditor();
        e.AddDigit(1); e.AddDigit(2); e.AddDigit(3);
        Assert.Equal("123", e.Number);
    }

    [Fact]
    public void AddDigit_InvalidForBase_Ignored()
    {
        // В двоичной '2' недопустима
        var e = new PEditor(2);
        e.AddDigit(1);
        e.AddDigit(2); // должна быть проигнорирована
        Assert.Equal("1", e.Number);
    }

    [Fact]
    public void AddDigit_HexInBase16_AcceptsAF()
    {
        var e = new PEditor(16);
        e.AddDigit(10); // A
        e.AddDigit(11); // B
        Assert.Equal("AB", e.Number);
    }

    // ── AddSign ──────────────────────────────────────────────────────────────

    [Fact]
    public void AddSign_PositiveNumber_AddsMinusSign()
    {
        var e = new PEditor();
        e.AddDigit(5);
        e.AddSign();
        Assert.Equal("-5", e.Number);
    }

    [Fact]
    public void AddSign_NegativeNumber_RemovesMinusSign()
    {
        var e = new PEditor();
        e.AddDigit(5);
        e.AddSign(); // -5
        e.AddSign(); // 5
        Assert.Equal("5", e.Number);
    }

    [Fact]
    public void AddSign_Zero_NoEffect()
    {
        var e = new PEditor();
        e.AddSign();
        Assert.Equal("0", e.Number); // "-0" не должно появляться
    }

    // ── AddSeparator (точка) ─────────────────────────────────────────────────

    [Fact]
    public void AddSeparator_Dot_AddsDotToNumber()
    {
        var e = new PEditor();
        e.AddDigit(3);
        e.AddSeparator(EditorCmd.SepDot);
        e.AddDigit(1);
        e.AddDigit(4);
        Assert.Equal("3.14", e.Number);
    }

    [Fact]
    public void AddSeparator_TwoDots_SecondIgnored()
    {
        var e = new PEditor();
        e.AddDigit(3);
        e.AddSeparator(EditorCmd.SepDot);
        e.AddSeparator(EditorCmd.SepDot); // вторая точка игнорируется
        e.AddDigit(1);
        Assert.Equal("3.1", e.Number);
    }

    // ── BackSpace ────────────────────────────────────────────────────────────

    [Fact]
    public void BackSpace_MultiDigit_RemovesLastDigit()
    {
        var e = new PEditor();
        e.AddDigit(1); e.AddDigit(2); e.AddDigit(3);
        e.BackSpace();
        Assert.Equal("12", e.Number);
    }

    [Fact]
    public void BackSpace_SingleDigit_ReturnsZero()
    {
        var e = new PEditor();
        e.AddDigit(5);
        e.BackSpace();
        Assert.Equal("0", e.Number);
    }

    [Fact]
    public void BackSpace_AfterDot_RemovesDotAndResetFlag()
    {
        var e = new PEditor();
        e.AddDigit(3);
        e.AddSeparator(EditorCmd.SepDot);
        e.BackSpace(); // удаляем точку
        e.AddSeparator(EditorCmd.SepDot); // должна снова добавиться
        Assert.Equal("3.", e.Number);
    }

    // ── Clear ────────────────────────────────────────────────────────────────

    [Fact]
    public void Clear_Always_ReturnsZero()
    {
        var e = new PEditor();
        e.AddDigit(1); e.AddDigit(2);
        e.Clear();
        Assert.Equal("0", e.Number);
        Assert.True(e.IsZero());
    }
}

public class CEditorTests
{
    [Fact]
    public void Initial_NumberIsZero()
    {
        Assert.Equal("0", new CEditor().Number);
    }

    [Fact]
    public void AddDigit_WithoutI_BuildsRealPart()
    {
        var e = new CEditor();
        e.AddDigit(3); e.AddDigit(4);
        Assert.Equal("34", e.Number);
    }

    [Fact]
    public void AddI_SwitchesToImaginaryPart()
    {
        var e = new CEditor();
        e.AddDigit(3);
        e.AddSeparator(EditorCmd.SepComplex); // нажали "i"
        e.AddDigit(4);
        Assert.Equal("3+4i", e.Number);
    }

    [Fact]
    public void AddI_NegativeImaginary_FormatsCorrectly()
    {
        var e = new CEditor();
        e.AddDigit(3);
        e.AddSeparator(EditorCmd.SepComplex); // переключились на im
        e.AddDigit(4);
        e.AddSign();   // меняем знак уже введённой цифры
        Assert.Equal("3-4i", e.Number);
    }

    [Fact]
    public void AddI_Twice_SecondIgnored()
    {
        var e = new CEditor();
        e.AddDigit(3);
        e.AddSeparator(EditorCmd.SepComplex);
        e.AddSeparator(EditorCmd.SepComplex); // второй "i" игнорируется
        e.AddDigit(4);
        Assert.Equal("3+4i", e.Number);
    }

    [Fact]
    public void BackSpace_RemovesFromImaginary()
    {
        var e = new CEditor();
        e.AddDigit(3);
        e.AddSeparator(EditorCmd.SepComplex);
        e.AddDigit(4);
        e.BackSpace(); // "4" удалили → im стал "0"
        e.BackSpace(); // im == "0" → выходим к re
        Assert.Equal("3", e.Number);
    }

    [Fact]
    public void AddSign_OnRealPart_NegatesRe()
    {
        var e = new CEditor();
        e.AddDigit(3);
        e.AddSign();
        Assert.Equal("-3", e.Number);
    }

    [Fact]
    public void Clear_ResetsEverything()
    {
        var e = new CEditor();
        e.AddDigit(3);
        e.AddSeparator(EditorCmd.SepComplex);
        e.AddDigit(4);
        e.Clear();
        Assert.Equal("0", e.Number);
        Assert.True(e.IsZero());
    }

    [Fact]
    public void SetNumber_ComplexString_ParsedCorrectly()
    {
        var e = new CEditor();
        e.Number = "5-2i";
        Assert.Equal("5-2i", e.Number);
    }
}


// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
//  ТЕСТЫ: TMemory — память калькулятора
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
public class TMemoryTests
{
    private TMemory MakeMemory() => new TMemory(new TComp(0, 0));

    [Fact]
    public void Initial_StateIsOff()
    {
        var m = MakeMemory();
        Assert.Equal(MemoryState.Off, m.State);
        Assert.Equal("", m.StateString);
    }

    [Fact]
    public void Store_TurnsStateOn()
    {
        var m = MakeMemory();
        m.Store(new TComp(3, 4));
        Assert.Equal(MemoryState.On, m.State);
        Assert.Equal("M", m.StateString);
    }

    [Fact]
    public void Restore_ReturnsStoredValue()
    {
        var m = MakeMemory();
        m.Store(new TComp(3, 4));
        var r = (TComp)m.Restore();
        Assert.Equal(3, r.Re, precision: 10);
        Assert.Equal(4, r.Im, precision: 10);
    }

    [Fact]
    public void Restore_ReturnsCopy_NotReference()
    {
        var m = MakeMemory();
        var stored = new TComp(3, 4);
        m.Store(stored);
        var r1 = m.Restore();
        var r2 = m.Restore();
        Assert.False(ReferenceEquals(r1, r2)); // каждый Restore даёт новый объект
    }

    [Fact]
    public void Add_AddsToStoredValue()
    {
        // В памяти 3+4i, добавляем 1+2i → должно стать 4+6i
        var m = MakeMemory();
        m.Store(new TComp(3, 4));
        m.Add(new TComp(1, 2));
        var r = (TComp)m.Restore();
        Assert.Equal(4, r.Re, precision: 10);
        Assert.Equal(6, r.Im, precision: 10);
    }

    [Fact]
    public void Add_WhenEmpty_TurnsStateOn()
    {
        var m = MakeMemory();
        m.Add(new TComp(5, 0));
        Assert.Equal(MemoryState.On, m.State);
    }

    [Fact]
    public void Clear_TurnsStateOff()
    {
        var m = MakeMemory();
        m.Store(new TComp(3, 4));
        m.Clear(new TComp(0, 0));
        Assert.Equal(MemoryState.Off, m.State);
        Assert.Equal("", m.StateString);
    }

    [Fact]
    public void Clear_ResetsStoredValue()
    {
        var m = MakeMemory();
        m.Store(new TComp(3, 4));
        m.Clear(new TComp(0, 0));
        var r = (TComp)m.Restore();
        Assert.True(r.EqZero());
    }
}


// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
//  ТЕСТЫ: TProc — процессор вычислений
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
public class TProcTests
{
    private TProc MakeProc() => new TProc(new TComp(0, 0), new TComp(0, 0));

    [Fact]
    public void OprtnRun_None_DoesNothing()
    {
        var p = MakeProc();
        p.SetLop(new TComp(5, 0));
        p.SetRop(new TComp(3, 0));
        p.Operation = TOprtn.None;
        p.OprtnRun();
        // lop должен остаться 5+0i
        Assert.Equal("5+0i", p.Lop.NumberString);
    }

    [Fact]
    public void OprtnRun_Add_UpdatesLop()
    {
        // lop=3+4i, rop=1+2i, op=Add → lop должен стать 4+6i
        var p = MakeProc();
        p.SetLop(new TComp(3, 4));
        p.SetRop(new TComp(1, 2));
        p.Operation = TOprtn.Add;
        p.OprtnRun();
        var r = (TComp)p.Lop;
        Assert.Equal(4, r.Re, precision: 10);
        Assert.Equal(6, r.Im, precision: 10);
    }

    [Fact]
    public void OprtnRun_Sub_UpdatesLop()
    {
        var p = MakeProc();
        p.SetLop(new TComp(5, 3));
        p.SetRop(new TComp(2, 1));
        p.Operation = TOprtn.Sub;
        p.OprtnRun();
        var r = (TComp)p.Lop;
        Assert.Equal(3, r.Re, precision: 10);
        Assert.Equal(2, r.Im, precision: 10);
    }

    [Fact]
    public void OprtnRun_Mul_UpdatesLop()
    {
        var p = MakeProc();
        p.SetLop(new TComp(2, 3));
        p.SetRop(new TComp(1, 4));
        p.Operation = TOprtn.Mul;
        p.OprtnRun();
        var r = (TComp)p.Lop;
        Assert.Equal(-10, r.Re, precision: 10);
        Assert.Equal(11,  r.Im, precision: 10);
    }

    [Fact]
    public void OprtnRun_DivByZero_SetsErrorMessage()
    {
        var p = MakeProc();
        p.SetLop(new TComp(1, 0));
        p.SetRop(new TComp(0, 0));
        p.Operation = TOprtn.Dvd;
        p.OprtnRun();
        Assert.NotEmpty(p.Error); // должно появиться сообщение об ошибке
    }

    [Fact]
    public void FuncRun_Sqr_UpdatesRop()
    {
        // rop = 3+4i, Sqr → rop должен стать -7+24i
        var p = MakeProc();
        p.SetRop(new TComp(3, 4));
        p.FuncRun(TFunc.Sqr);
        var r = (TComp)p.Rop;
        Assert.Equal(-7, r.Re, precision: 10);
        Assert.Equal(24, r.Im, precision: 10);
    }

    [Fact]
    public void FuncRun_Rev_UpdatesRop()
    {
        // rop = 0+1i (i), Rev → 0-1i (-i)
        var p = MakeProc();
        p.SetRop(new TComp(0, 1));
        p.FuncRun(TFunc.Rev);
        var r = (TComp)p.Rop;
        Assert.Equal(0,  r.Re, precision: 10);
        Assert.Equal(-1, r.Im, precision: 10);
    }

    [Fact]
    public void FuncRun_Modulus_ReturnsRealNumber()
    {
        // |3+4i| = 5
        var p = MakeProc();
        p.SetRop(new TComp(3, 4));
        p.FuncRun(TFunc.Mdl);
        var r = (TComp)p.Rop;
        Assert.Equal(5, r.Re, precision: 10);
        Assert.Equal(0, r.Im, precision: 10); // мнимая часть = 0
    }

    [Fact]
    public void FuncRun_Root_PopulatesLastRoots()
    {
        // 4 корня из 1
        var p = MakeProc();
        p.SetRop(new TComp(1, 0));
        p.FuncRun(TFunc.Root, 4);
        Assert.Equal(4, p.LastRoots.Count);
    }

    [Fact]
    public void OprtnClear_SetsOperationToNone()
    {
        var p = MakeProc();
        p.Operation = TOprtn.Add;
        p.OprtnClear();
        Assert.Equal(TOprtn.None, p.Operation);
    }

    [Fact]
    public void ReSet_ClearsAllState()
    {
        var p = MakeProc();
        p.SetLop(new TComp(5, 5));
        p.SetRop(new TComp(5, 5));
        p.Operation = TOprtn.Add;
        p.ReSet(new TComp(0, 0));

        Assert.Equal(TOprtn.None, p.Operation);
        Assert.Equal("", p.Error);
        Assert.True(((TComp)p.Lop).EqZero());
        Assert.True(((TComp)p.Rop).EqZero());
    }

    [Fact]
    public void SetLop_StoresCopy_NotReference()
    {
        var p = MakeProc();
        var n = new TComp(3, 4);
        p.SetLop(n);
        // Изменяем оригинал через NumberString — lop не должен измениться
        Assert.False(ReferenceEquals(n, p.Lop));
    }
}


// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
//  ТЕСТЫ: TCtrl — контроллер (интеграционные тесты)
// ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
public class TCtrlTests
{
    // Вспомогательный метод: создаёт контроллер и выполняет команды
    private TCtrl MakeCtrl() => new TCtrl(CalcMode.Complex, 10);

    private string Exec(TCtrl c, int cmd)
    {
        return c.ExecCmd(cmd, out _);
    }

    // ── Начальное состояние ──────────────────────────────────────────────────

    [Fact]
    public void Initial_StateIsStart()
    {
        var c = MakeCtrl();
        Assert.Equal(CtrlState.Start, c.State);
    }

    [Fact]
    public void Initial_MemoryIsEmpty()
    {
        var c = MakeCtrl();
        Assert.Equal("", c.MemoryState);
    }

    // ── Ввод цифр ────────────────────────────────────────────────────────────

    [Fact]
    public void PressDigit_ShowsDigitOnDisplay()
    {
        var c = MakeCtrl();
        var r = Exec(c, CalcCmd.D5);
        Assert.Equal("5", r);
    }

    [Fact]
    public void PressDigits_BuildsNumber()
    {
        var c = MakeCtrl();
        Exec(c, CalcCmd.D1);
        Exec(c, CalcCmd.D2);
        var r = Exec(c, CalcCmd.D3);
        Assert.Equal("123", r);
    }

    [Fact]
    public void PressDigit_AfterResult_StartsNewNumber()
    {
        // После "5 + 3 =" нажимаем "7" → дисплей должен показать "7", а не "87"
        var c = MakeCtrl();
        Exec(c, CalcCmd.D5);
        Exec(c, CalcCmd.OpAdd);
        Exec(c, CalcCmd.D3);
        Exec(c, CalcCmd.Equal);
        var r = Exec(c, CalcCmd.D7);
        Assert.Equal("7", r);
    }

    // ── Базовая арифметика ───────────────────────────────────────────────────

    [Fact]
    public void Add_FivePlusThree_ReturnsEight()
    {
        var c = MakeCtrl();
        Exec(c, CalcCmd.D5);
        Exec(c, CalcCmd.OpAdd);
        Exec(c, CalcCmd.D3);
        var r = Exec(c, CalcCmd.Equal);
        Assert.Contains("8", r);
    }

    [Fact]
    public void Sub_SevenMinusThree_ReturnsFour()
    {
        var c = MakeCtrl();
        Exec(c, CalcCmd.D7);
        Exec(c, CalcCmd.OpSub);
        Exec(c, CalcCmd.D3);
        var r = Exec(c, CalcCmd.Equal);
        Assert.Contains("4", r);
    }

    [Fact]
    public void Mul_ThreeTimesThree_ReturnsNine()
    {
        var c = MakeCtrl();
        Exec(c, CalcCmd.D3);
        Exec(c, CalcCmd.OpMul);
        Exec(c, CalcCmd.D3);
        var r = Exec(c, CalcCmd.Equal);
        Assert.Contains("9", r);
    }

    [Fact]
    public void Div_TenDividedByTwo_ReturnsFive()
    {
        var c = MakeCtrl();
        Exec(c, CalcCmd.D1); Exec(c, CalcCmd.D0);
        Exec(c, CalcCmd.OpDvd);
        Exec(c, CalcCmd.D2);
        var r = Exec(c, CalcCmd.Equal);
        Assert.Contains("5", r);
    }

    [Fact]
    public void Div_ByZero_ReturnsErrorMessage()
    {
        var c = MakeCtrl();
        Exec(c, CalcCmd.D5);
        Exec(c, CalcCmd.OpDvd);
        Exec(c, CalcCmd.D0);
        var r = Exec(c, CalcCmd.Equal);
        // Результат должен содержать сообщение об ошибке, не число
        Assert.NotEqual("0", r);
        Assert.Equal(CtrlState.Error, c.State);
    }

    // ── Повторное нажатие "=" ─────────────────────────────────────────────────

    [Fact]
    public void Equal_Repeated_RepeatsLastOperation()
    {
        // 5 + 3 = 8, = 11, = 14
        var c = MakeCtrl();
        Exec(c, CalcCmd.D5);
        Exec(c, CalcCmd.OpAdd);
        Exec(c, CalcCmd.D3);
        Exec(c, CalcCmd.Equal);           // 8
        var r1 = Exec(c, CalcCmd.Equal);  // 11
        var r2 = Exec(c, CalcCmd.Equal);  // 14
        Assert.Contains("11", r1);
        Assert.Contains("14", r2);
    }

    // ── Функции ──────────────────────────────────────────────────────────────

    [Fact]
    public void FnSqr_Four_ReturnsSixteen()
    {
        var c = MakeCtrl();
        Exec(c, CalcCmd.D4);
        var r = Exec(c, CalcCmd.FnSqr);
        Assert.Contains("16", r);
    }

    [Fact]
    public void FnRev_Two_ReturnsHalf()
    {
        var c = MakeCtrl();
        Exec(c, CalcCmd.D2);
        var r = Exec(c, CalcCmd.FnRev);
        Assert.Contains("0.5", r);
    }

    [Fact]
    public void FnRev_Zero_ReturnsError()
    {
        var c = MakeCtrl();
        Exec(c, CalcCmd.D0);
        var r = Exec(c, CalcCmd.FnRev);
        Assert.Equal(CtrlState.Error, c.State);
    }

    // ── BackSpace и ClearEntry ────────────────────────────────────────────────

    [Fact]
    public void BackSpace_RemovesLastDigit()
    {
        var c = MakeCtrl();
        Exec(c, CalcCmd.D1); Exec(c, CalcCmd.D2); Exec(c, CalcCmd.D3);
        var r = Exec(c, CalcCmd.BackSpace);
        Assert.Equal("12", r);
    }

    [Fact]
    public void ClearEntry_ResetsCurrentInput()
    {
        var c = MakeCtrl();
        Exec(c, CalcCmd.D5); Exec(c, CalcCmd.D5); Exec(c, CalcCmd.D5);
        var r = Exec(c, CalcCmd.ClearEntry);
        Assert.Equal("0", r);
    }

    [Fact]
    public void ClearAll_ResetsEverything()
    {
        var c = MakeCtrl();
        Exec(c, CalcCmd.D5);
        Exec(c, CalcCmd.OpAdd);
        Exec(c, CalcCmd.D3);
        Exec(c, CalcCmd.ClearAll);
        Assert.Equal(CtrlState.Start, c.State);
    }

    // ── Память ───────────────────────────────────────────────────────────────

    [Fact]
    public void Memory_StoreAndRestore_ReturnsStoredValue()
    {
        var c = MakeCtrl();
        Exec(c, CalcCmd.D5);
        c.ExecCmd(CalcCmd.MemStore, out _);
        Exec(c, CalcCmd.ClearAll);
        var r = c.ExecCmd(CalcCmd.MemRestore, out string mem);
        Assert.Contains("5", r);
        Assert.Equal("M", mem);
    }

    [Fact]
    public void Memory_Clear_RemovesIndicator()
    {
        var c = MakeCtrl();
        Exec(c, CalcCmd.D5);
        c.ExecCmd(CalcCmd.MemStore, out _);
        c.ExecCmd(CalcCmd.MemClear, out string mem);
        Assert.Equal("", mem);
    }

    [Fact]
    public void Memory_Add_AccumulatesValues()
    {
        // MS(3), M+(4) → в памяти должно быть 7
        var c = MakeCtrl();
        Exec(c, CalcCmd.D3);
        c.ExecCmd(CalcCmd.MemStore, out _);   // MS: память = 3
        Exec(c, CalcCmd.ClearAll);
        Exec(c, CalcCmd.D4);
        c.ExecCmd(CalcCmd.MemAdd, out _);     // M+: память = 7
        var r = c.ExecCmd(CalcCmd.MemRestore, out _);
        Assert.Contains("7", r);
    }

    // ── Смена режима ─────────────────────────────────────────────────────────

    [Fact]
    public void SetMode_Complex_ResetsStateToStart()
    {
        // После SetMode контроллер должен сброситься в начальное состояние
        var c = MakeCtrl();
        Exec(c, CalcCmd.D5);
        Exec(c, CalcCmd.OpAdd);
        c.SetMode(CalcMode.Complex, 10);
        Assert.Equal(CtrlState.Start, c.State);
    }

    [Fact]
    public void SetMode_Complex_WorksWithComplexNumbers()
    {
        // 4+0i + 3+0i = 7+0i
        var c = MakeCtrl();
        c.SetMode(CalcMode.Complex, 10);
        Exec(c, CalcCmd.D4);
        Exec(c, CalcCmd.OpAdd);
        Exec(c, CalcCmd.D3);
        var r = Exec(c, CalcCmd.Equal);
        Assert.Contains("7", r);
    }

    // ── Состояния автомата ───────────────────────────────────────────────────

    [Fact]
    public void State_AfterDigit_IsEditing()
    {
        var c = MakeCtrl();
        Exec(c, CalcCmd.D5);
        Assert.Equal(CtrlState.Editing, c.State);
    }

    [Fact]
    public void State_AfterOperation_IsValDone()
    {
        var c = MakeCtrl();
        Exec(c, CalcCmd.D5);
        Exec(c, CalcCmd.OpAdd);
        Assert.Equal(CtrlState.ValDone, c.State);
    }

    [Fact]
    public void State_AfterEqual_IsExpDone()
    {
        var c = MakeCtrl();
        Exec(c, CalcCmd.D5);
        Exec(c, CalcCmd.OpAdd);
        Exec(c, CalcCmd.D3);
        Exec(c, CalcCmd.Equal);
        Assert.Equal(CtrlState.ExpDone, c.State);
    }

    [Fact]
    public void State_AfterFunc_IsFunDone()
    {
        var c = MakeCtrl();
        Exec(c, CalcCmd.D4);
        Exec(c, CalcCmd.FnSqr);
        Assert.Equal(CtrlState.FunDone, c.State);
    }

    [Fact]
    public void State_AfterClearAll_IsStart()
    {
        var c = MakeCtrl();
        Exec(c, CalcCmd.D5);
        Exec(c, CalcCmd.ClearAll);
        Assert.Equal(CtrlState.Start, c.State);
    }
}