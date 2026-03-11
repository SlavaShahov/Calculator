using System;
using UniversalCaluclator.Models.Editors;
using UniversalCaluclator.Models.Memory;
using UniversalCaluclator.Models.Numbers;
using UniversalCaluclator.Models.Processor;

namespace UniversalCaluclator.Models.Controller;

public enum CalcMode  { PNumber, Fraction, Complex }
public enum CtrlState { Start, Editing, ValDone, ExpDone, FunDone, OpChange, Error }

public static class CalcCmd
{
    public const int D0         = 0;
    public const int D1         = 1;
    public const int D2         = 2;
    public const int D3         = 3;
    public const int D4         = 4;
    public const int D5         = 5;
    public const int D6         = 6;
    public const int D7         = 7;
    public const int D8         = 8;
    public const int D9         = 9;
    public const int DA         = 10;
    public const int DB         = 11;
    public const int DC         = 12;
    public const int DD         = 13;
    public const int DE         = 14;
    public const int DF         = 15;
    public const int Sign       = 16;
    public const int SepFrac    = 17;
    public const int SepDot     = 18;
    public const int SepComplex = 19;
    public const int BackSpace  = 20;
    public const int ClearEntry = 21;
    public const int OpAdd      = 30;
    public const int OpSub      = 31;
    public const int OpMul      = 32;
    public const int OpDvd      = 33;
    public const int FnSqr      = 34;
    public const int FnRev      = 35;
    public const int Equal      = 36;
    public const int ClearAll   = 37;
    public const int MemStore   = 40;
    public const int MemRestore = 41;
    public const int MemClear   = 42;
    public const int MemAdd     = 43;
}

public class TCtrl
{
    private AEditor  _editor = null!;
    private TProc    _proc   = null!;
    private TMemory  _memory = null!;
    private CtrlState _state;
    private CalcMode  _mode;
    private int       _pBase;
    private TANumber  _lastRop = null!;
    private TOprtn    _lastOp;

    public CtrlState State       => _state;
    public string    MemoryState => _memory.StateString;

    public TCtrl(CalcMode mode = CalcMode.PNumber, int pBase = 10)
    {
        _mode  = mode;
        _pBase = pBase;
        Init();
    }

    private void Init()
    {
        TANumber zero = MakeZero();
        _editor  = MakeEditor();
        _proc    = new TProc(zero, zero);
        _memory  = new TMemory(zero);
        _state   = CtrlState.Start;
        _lastRop = zero;
        _lastOp  = TOprtn.None;
    }

    private TANumber MakeZero() => _mode switch
    {
        CalcMode.Fraction => new TFrac(0, 1),
        CalcMode.Complex  => new TComp(0, 0),
        _                 => new TPNumber(0, _pBase)
    };

    private AEditor MakeEditor() => _mode switch
    {
        CalcMode.Fraction => new FEditor(),
        CalcMode.Complex  => new CEditor(),
        _                 => new PEditor(_pBase)
    };

    private TANumber ParseEditor()
    {
        string   s = _editor.Number;
        TANumber n = MakeZero();
        n.NumberString = s;
        return n;
    }

    public void SetMode(CalcMode mode, int pBase = 10)
    {
        _mode  = mode;
        _pBase = pBase;
        Init();
    }

    // ── Главная точка входа ────────────────────────────────────────────────
    public string ExecCmd(int cmd, out string memState)
    {
        string result;

        if (cmd is >= CalcCmd.D0 and <= EditorCmd.Clear)
            result = ExecEditorCmd(cmd);
        else if (cmd is >= CalcCmd.OpAdd and <= CalcCmd.Equal)
            result = ExecCalcCmd(cmd);
        else if (cmd == CalcCmd.ClearAll)
            result = ExecReset();
        else if (cmd is >= CalcCmd.MemStore and <= CalcCmd.MemAdd)
            result = ExecMemCmd(cmd);
        else
            result = _editor.Number;

        memState = _memory.StateString;
        return result;
    }

    // ── Редактор ───────────────────────────────────────────────────────────
    private string ExecEditorCmd(int cmd)
    {
        if (_state is CtrlState.ValDone or CtrlState.ExpDone or CtrlState.FunDone)
        {
            _editor.Clear();
            _state = CtrlState.Editing;
        }
        else if (_state == CtrlState.Start)
        {
            _state = CtrlState.Editing;
        }

        string r = _editor.Edit(cmd);
        if (_state != CtrlState.Error) _state = CtrlState.Editing;
        return r;
    }

    // ── Вычисления ─────────────────────────────────────────────────────────
    private string ExecCalcCmd(int cmd)
    {
        if (cmd == CalcCmd.Equal)          return ExecEquals();
        if (cmd is CalcCmd.FnSqr or CalcCmd.FnRev) return ExecFunc(cmd);
        return ExecOp(cmd);
    }

    private string ExecOp(int cmd)
    {
        TOprtn op = cmd switch
        {
            CalcCmd.OpAdd => TOprtn.Add,
            CalcCmd.OpSub => TOprtn.Sub,
            CalcCmd.OpMul => TOprtn.Mul,
            CalcCmd.OpDvd => TOprtn.Dvd,
            _             => TOprtn.None
        };

        if (_state == CtrlState.Editing || _state == CtrlState.Start)
        {
            var n = ParseEditor();
            if (_proc.Operation != TOprtn.None)
            {
                _proc.SetRop(n);
                _proc.OprtnRun();
                if (_proc.Error != "") { _state = CtrlState.Error; return _proc.Error; }
            }
            else
            {
                _proc.SetLop(n);
            }
        }
        else if (_state == CtrlState.FunDone)
        {
            if (_proc.Operation != TOprtn.None)
            {
                _proc.OprtnRun();
                if (_proc.Error != "") { _state = CtrlState.Error; return _proc.Error; }
            }
            else
            {
                _proc.SetLop(_proc.Rop);
            }
        }
        else if (_state is CtrlState.ExpDone or CtrlState.ValDone)
        {
            // SKIP — только меняем операцию
        }
        else if (_state == CtrlState.OpChange)
        {
            _proc.Operation = op;
            _state = CtrlState.OpChange;
            return _proc.Lop.NumberString;
        }

        _proc.Operation  = op;
        _editor.Number   = _proc.Lop.NumberString;
        _state           = CtrlState.ValDone;
        return _proc.Lop.NumberString;
    }

    private string ExecFunc(int cmd)
    {
        var n = _state == CtrlState.Editing ? ParseEditor() : _proc.Lop;
        _proc.SetRop(n);
        _proc.FuncRun(cmd == CalcCmd.FnSqr ? TFunc.Sqr : TFunc.Rev);

        if (_proc.Error != "") { _state = CtrlState.Error; return _proc.Error; }

        if (_proc.Operation == TOprtn.None)
            _proc.SetLop(_proc.Rop);

        _editor.Number = _proc.Rop.NumberString;
        _state         = CtrlState.FunDone;
        return _proc.Rop.NumberString;
    }

    private string ExecEquals()
    {
        // Повторное "=" — применяем последнюю сохранённую операцию
        if (_state == CtrlState.ExpDone)
        {
            if (_lastOp == TOprtn.None) return _editor.Number;

            _proc.Operation = _lastOp;
            _proc.SetRop(_lastRop);
            _proc.OprtnRun();
            if (_proc.Error != "") { _state = CtrlState.Error; return _proc.Error; }

            _editor.Number = _proc.Lop.NumberString;
            _proc.OprtnClear();
            _state = CtrlState.ExpDone;
            return _proc.Lop.NumberString;
        }

        if (_state == CtrlState.FunDone && _proc.Operation == TOprtn.None)
        {
            _lastOp = TOprtn.None;
            _state  = CtrlState.ExpDone;
            return _editor.Number;
        }

        TANumber rop;
        if (_state == CtrlState.FunDone)
            rop = _proc.Rop;
        else if (_state == CtrlState.ValDone)
            rop = _proc.Lop.Copy();
        else
        {
            try   { rop = ParseEditor(); }
            catch { rop = _proc.Lop.Copy(); }
        }

        _lastRop = rop.Copy();
        _lastOp  = _proc.Operation;

        _proc.SetRop(rop);
        _proc.OprtnRun();
        if (_proc.Error != "") { _state = CtrlState.Error; return _proc.Error; }

        _editor.Number = _proc.Lop.NumberString;
        _proc.OprtnClear();
        _state = CtrlState.ExpDone;
        return _proc.Lop.NumberString;
    }

    private string ExecReset()
    {
        _proc.ReSet(MakeZero());
        _editor.Clear();
        _lastRop = MakeZero();
        _lastOp  = TOprtn.None;
        _state   = CtrlState.Start;
        return "0";
    }

    // ── Память ─────────────────────────────────────────────────────────────
    private string ExecMemCmd(int cmd)
    {
        var n = ParseEditor();
        switch (cmd)
        {
            case CalcCmd.MemStore:
                _memory.Store(n);
                break;
            case CalcCmd.MemAdd:
                _memory.Add(n);
                break;
            case CalcCmd.MemRestore:
                var restored = _memory.Restore();
                _editor.Number = restored.NumberString;
                _proc.SetRop(restored);
                _state = CtrlState.Editing;
                return _editor.Number;
            case CalcCmd.MemClear:
                _memory.Clear(MakeZero());
                break;
        }
        return _editor.Number;
    }
}