namespace UniversalCaluclator.Models.Editors;

public static class EditorCmd
{
    public const int D0 = 0;
    public const int D1 = 1;
    public const int D2 = 2;
    public const int D3 = 3;
    public const int D4 = 4;
    public const int D5 = 5;
    public const int D6 = 6;
    public const int D7 = 7;
    public const int D8 = 8;
    public const int D9 = 9;
    public const int DA = 10;
    public const int DB = 11;
    public const int DC = 12;
    public const int DD = 13;
    public const int DE = 14;
    public const int DF = 15;
    public const int Sign      = 16;
    public const int SepFrac   = 17;
    public const int SepDot    = 18;
    public const int SepComplex= 19;
    public const int BackSpace = 20;
    public const int Clear     = 21;
}

public abstract class AEditor
{
    protected string FNumber = "0";

    public virtual string Number
    {
        get => GetNum();
        set => SetNum(value);
    }

    protected abstract string GetNum();
    protected abstract void SetNum(string n);

    public abstract bool IsZero();
    public abstract string AddDigit(int d);
    public abstract string AddSign();
    public abstract string AddSeparator(int sepType);
    public abstract string BackSpace();
    public abstract string Clear();

    public virtual string Edit(int cmd)
    {
        if (cmd is >= 0 and <= 15) return AddDigit(cmd);
        return cmd switch
        {
            EditorCmd.Sign       => AddSign(),
            EditorCmd.SepFrac    => AddSeparator(EditorCmd.SepFrac),
            EditorCmd.SepDot     => AddSeparator(EditorCmd.SepDot),
            EditorCmd.SepComplex => AddSeparator(EditorCmd.SepComplex),
            EditorCmd.BackSpace  => BackSpace(),
            EditorCmd.Clear      => Clear(),
            _ => FNumber
        };
    }
}
