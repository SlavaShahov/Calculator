namespace UniversalCaluclator.Models.Editors;

public class CEditor : AEditor
{
    private readonly PEditor _re;
    private readonly PEditor _im;
    private bool _editingIm;

    public CEditor()
    {
        _re = new PEditor(10);
        _im = new PEditor(10);
        _editingIm = false;
        FNumber = "0";
    }

    // Всегда возвращает "a+bi" — форматирование только в ViewModel
    protected override string GetNum()
    {
        string re = _re.Number;
        string im = _im.Number;

        if (!_editingIm)
            return re;

        string sign = im.StartsWith('-') ? "" : "+";
        return $"{re}{sign}{im}i";
    }

    protected override void SetNum(string n)
    {
        FNumber = n;
        int iIdx = n.LastIndexOf('i');
        if (iIdx < 0)
        {
            _re.Number = n;
            _im.Number = "0";
            _editingIm = false;
            return;
        }

        string body = n[..iIdx].Trim();
        int sep = -1;
        for (int k = body.Length - 1; k > 0; k--)
            if (body[k] == '+' || body[k] == '-') { sep = k; break; }

        if (sep < 0)
        {
            _re.Number = "0";
            string ip = body.Trim();
            _im.Number = ip is "" or "+" ? "1" : ip == "-" ? "-1" : ip;
        }
        else
        {
            _re.Number = body[..sep].Trim();
            string ip = body[sep..].Trim();
            _im.Number = ip == "+" ? "1" : ip == "-" ? "-1" : ip;
        }
        _editingIm = true;
    }

    public override bool IsZero() => _re.IsZero() && _im.IsZero();

    public override string AddDigit(int d)
    {
        if (_editingIm) _im.AddDigit(d);
        else            _re.AddDigit(d);
        return GetNum();
    }

    public override string AddSign()
    {
        if (_editingIm) _im.AddSign();
        else            _re.AddSign();
        return GetNum();
    }

    public override string AddSeparator(int sepType)
    {
        if (sepType == EditorCmd.SepComplex)
        {
            if (!_editingIm)
            {
                _im.Clear();       // im = "0"
                _editingIm = true;
            }
            // повторное i — игнорируем
        }
        else
        {
            if (_editingIm) _im.AddSeparator(sepType);
            else            _re.AddSeparator(sepType);
        }
        return GetNum();
    }

    public override string BackSpace()
    {
        if (_editingIm)
        {
            string cur = _im.Number;
            if (cur == "0")
                _editingIm = false;  // стёрли нулевую im — выходим к re
            else
                _im.BackSpace();     // PEditor сам вернёт "0" когда останется 1 цифра
        }
        else
        {
            _re.BackSpace();
        }
        return GetNum();
    }

    public override string Clear()
    {
        _re.Clear();
        _im.Clear();
        _editingIm = false;
        return "0";
    }
}