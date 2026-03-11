namespace UniversalCaluclator.Models.Editors;

public class FEditor : AEditor
{
    protected bool HasSep = false;

    public FEditor() { FNumber = "0"; }

    protected override string GetNum() => FNumber == "" ? "0" : FNumber;
    protected override void SetNum(string n) { FNumber = n; HasSep = FNumber.Contains('/'); }

    public override bool IsZero() => FNumber == "" || FNumber == "0";

    public override string AddDigit(int d)
    {
        if (d > 9) return GetNum();
        char ch = (char)('0' + d);
        if (FNumber == "0" && !HasSep)
            FNumber = ch.ToString();
        else
            FNumber += ch;
        return GetNum();
    }

    public override string AddSign()
    {
        if (FNumber.StartsWith('-'))
            FNumber = FNumber[1..];
        else if (FNumber != "0" && FNumber != "")
            FNumber = "-" + FNumber;
        return GetNum();
    }

    public override string AddSeparator(int sepType)
    {
        if (HasSep) return GetNum();
        if (sepType == EditorCmd.SepFrac)
        {
            if (FNumber == "" || FNumber == "0") FNumber = "0";
            FNumber += "/";
            HasSep = true;
        }
        return GetNum();
    }

    public override string BackSpace()
    {
        if (FNumber.Length <= 1) { FNumber = "0"; HasSep = false; return "0"; }
        char last = FNumber[^1];
        FNumber = FNumber[..^1];
        if (last == '/') HasSep = false;
        return GetNum();
    }

    public override string Clear()
    {
        FNumber = "0";
        HasSep = false;
        return "0";
    }
}
