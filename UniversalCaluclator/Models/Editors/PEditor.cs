namespace UniversalCaluclator.Models.Editors;

public class PEditor : AEditor
{
    private int _base;
    private bool _hasDot;

    public int Base
    {
        get => _base;
        set => _base = value is >= 2 and <= 16 ? value : 10;
    }

    public PEditor(int numBase = 10) { _base = numBase; FNumber = "0"; }

    protected override string GetNum() => FNumber;
    protected override void SetNum(string n) { FNumber = n; _hasDot = n.Contains('.'); }

    public override bool IsZero() => FNumber == "0" || FNumber == "";

    private static char DigitChar(int d) => d < 10 ? (char)('0' + d) : (char)('A' + d - 10);

    public override string AddDigit(int d)
    {
        if (d >= _base) return FNumber;
        char ch = DigitChar(d);
        if (FNumber == "0" && !_hasDot)
            FNumber = ch.ToString();
        else
            FNumber += ch;
        return FNumber;
    }

    public override string AddSign()
    {
        if (FNumber.StartsWith('-'))
            FNumber = FNumber[1..];
        else if (FNumber != "0")
            FNumber = "-" + FNumber;
        return FNumber;
    }

    public override string AddSeparator(int sepType)
    {
        if (sepType == EditorCmd.SepDot && !_hasDot)
        {
            if (FNumber == "" || FNumber == "-") FNumber += "0";
            FNumber += ".";
            _hasDot = true;
        }
        return FNumber;
    }

    public override string BackSpace()
    {
        if (FNumber.Length <= 1) { FNumber = "0"; _hasDot = false; return "0"; }
        if (FNumber[^1] == '.') _hasDot = false;
        FNumber = FNumber[..^1];
        return FNumber;
    }

    public override string Clear()
    {
        FNumber = "0";
        _hasDot = false;
        return "0";
    }
}
