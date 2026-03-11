namespace UniversalCaluclator.Models.Numbers;

public abstract class TANumber
{
    public abstract TANumber Add(TANumber b);
    public abstract TANumber Sub(TANumber b);
    public abstract TANumber Mul(TANumber b);
    public abstract TANumber Div(TANumber b);
    public abstract TANumber Sqr();
    public abstract TANumber Rev();
    public abstract bool EqZero();
    public abstract bool EqNumber(TANumber b);
    public abstract TANumber Copy();
    public abstract string NumberString { get; set; }
}
