using System;
using System.Collections.Generic;
using UniversalCaluclator.Models.Numbers;

namespace UniversalCaluclator.Models.Processor;

public enum TOprtn { None, Add, Sub, Mul, Dvd }
public enum TFunc { Rev, Sqr, Pwr, Root, Mdl, CnrDeg, CnrRad }

public class TProc
{
    private TANumber _lop;
    private TANumber _rop;
    private TOprtn _operation;
    private string _error;
    private List<TANumber> _lastRoots; // Для хранения корней

    public TANumber Lop => _lop;
    public TANumber Rop => _rop;
    public TOprtn Operation { get => _operation; set => _operation = value; }
    public string Error => _error;
    public List<TANumber> LastRoots => _lastRoots;

    public TProc(TANumber l, TANumber r)
    {
        _lop = l.Copy();
        _rop = r.Copy();
        _operation = TOprtn.None;
        _error = "";
        _lastRoots = new List<TANumber>();
    }

    public void SetLop(TANumber operand) => _lop = operand.Copy();
    public void SetRop(TANumber operand) => _rop = operand.Copy();

    public void OprtnRun()
    {
        if (_operation == TOprtn.None) return;
        try
        {
            _lop = _operation switch
            {
                TOprtn.Add => _lop.Add(_rop),
                TOprtn.Sub => _lop.Sub(_rop),
                TOprtn.Mul => _lop.Mul(_rop),
                TOprtn.Dvd => _lop.Div(_rop),
                _ => _lop
            };
            _error = "";
        }
        catch (Exception ex)
        {
            _error = ex.Message;
        }
    }

    public void FuncRun(TFunc func, int parameter = 0)
    {
        try
        {
            _lastRoots.Clear();
            
            if (_rop is TComp comp)
            {
                switch (func)
                {
                    case TFunc.Pwr:
                        _rop = comp.Power(parameter);
                        break;
                    case TFunc.Root:
                        var roots = comp.Roots(parameter);
                        _lastRoots.AddRange(roots);
                        _rop = roots[0]; // Первый корень как основной результат
                        break;
                    case TFunc.Mdl:
                        // Создаем действительное число (комплексное с im=0)
                        _rop = new TComp(comp.Modulus, 0);
                        break;
                    case TFunc.CnrDeg:
                        _rop = new TComp(comp.ArgumentDeg, 0);
                        break;
                    case TFunc.CnrRad:
                        _rop = new TComp(comp.ArgumentRad, 0);
                        break;
                    case TFunc.Sqr:
                        _rop = comp.Sqr();
                        break;
                    case TFunc.Rev:
                        _rop = comp.Rev();
                        break;
                }
            }
            else
            {
                // Для не-комплексных чисел
                _rop = func switch
                {
                    TFunc.Sqr => _rop.Sqr(),
                    TFunc.Rev => _rop.Rev(),
                    _ => _rop
                };
            }
            _error = "";
        }
        catch (Exception ex)
        {
            _error = ex.Message;
        }
    }

    public void OprtnClear() => _operation = TOprtn.None;

    public void ReSet(TANumber zero)
    {
        _lop = zero.Copy();
        _rop = zero.Copy();
        _operation = TOprtn.None;
        _error = "";
        _lastRoots.Clear();
    }

    public void ClearError() => _error = "";
}