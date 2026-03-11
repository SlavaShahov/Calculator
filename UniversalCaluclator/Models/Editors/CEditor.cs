namespace UniversalCaluclator.Models.Editors;

public class CEditor : AEditor
{
    private readonly PEditor _re;
    private readonly PEditor _im;
    private bool _editingIm;
    // Флаг: пользователь только что нажал i, но ещё не ввёл ни одной цифры мнимой части.
    // В этом состоянии дисплей показывает "5i" (= 5+1i визуально, im ещё пуст).
    private bool _imPending;

    public CEditor()
    {
        _re = new PEditor(10);
        _im = new PEditor(10);
        _editingIm = false;
        _imPending = false;
        FNumber = "0";
    }

    // ── Отображение ────────────────────────────────────────────────────────
    protected override string GetNum()
    {
        string re = _re.Number;
        string im = _im.Number;

        if (!_editingIm)
            return re;

        // _imPending: i нажато, цифра мнимой ещё не введена
        // Показываем "5i" или "i" (подразумевается im=1 визуально, но не в im)
        if (_imPending)
            return re == "0" || re == "" ? "i" : $"{re}i";

        // im введена — строим нормальное отображение
        bool reIsZero  = re == "0" || re == "";
        bool imIsOne   = im == "1";
        bool imIsNegOne = im == "-1";
        bool imNeg     = im.StartsWith('-');

        // Абсолютная строка мнимой части без знака: "i", "3i", "0i"
        string imAbs = (imIsOne || imIsNegOne) ? "i" : $"{im.TrimStart('-')}i";

        if (reIsZero)
            return imNeg ? $"-{imAbs}" : imAbs;  // "0i", "i", "-i", "3i"

        string sep = imNeg ? "-" : "+";
        return $"{re}{sep}{imAbs}";              // "5+0i", "5+3i", "5-3i", "5+i"
    }

    protected override void SetNum(string n)
    {
        FNumber = n;
        _imPending = false;
        
        // Если строка не содержит 'i', значит это действительное число
        if (!n.Contains('i'))
        {
            _re.Number = n;
            _im.Number = "0";
            _editingIm = false;
            return;
        }

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

    // ── Цифра ──────────────────────────────────────────────────────────────
    public override string AddDigit(int d)
    {
        if (_editingIm)
        {
            if (_imPending)
            {
                // Первая цифра после i — начинаем мнимую с нуля
                _im.Number = "0";
                _imPending = false;
                _im.AddDigit(d);   // PEditor заменит "0" на цифру
            }
            else
            {
                _im.AddDigit(d);
            }
        }
        else
        {
            _re.AddDigit(d);
        }
        return GetNum();
    }

    // ── Знак ───────────────────────────────────────────────────────────────
    public override string AddSign()
    {
        if (_editingIm)
        {
            if (_imPending)
            {
                // Меняем знак "подразумеваемой 1": делаем im="-1" и выходим из pending
                _im.Number = "-1";
                _imPending = false;
            }
            else
            {
                _im.AddSign();
            }
        }
        else
        {
            _re.AddSign();
        }
        return GetNum();
    }

    // ── Разделители ────────────────────────────────────────────────────────
    public override string AddSeparator(int sepType)
    {
        if (sepType == EditorCmd.SepComplex)
        {
            if (!_editingIm)
            {
                // Нажали i впервые — переходим в pending режим
                _im.Clear();
                _editingIm = true;
                _imPending = true;
            }
            // повторное i — игнорируем
        }
        else
        {
            if (_editingIm && !_imPending) _im.AddSeparator(sepType);
            else if (!_editingIm)         _re.AddSeparator(sepType);
        }
        return GetNum();
    }

    // ── Backspace ──────────────────────────────────────────────────────────
    public override string BackSpace()
    {
        if (_editingIm)
        {
            if (_imPending)
            {
                // Стёрли i — возвращаемся к вещественной части
                _imPending = false;
                _editingIm = false;
            }
            else if (_im.Number.Length <= 1)
            {
                // Стёрли последнюю цифру мнимой — уходим в pending
                _im.Clear();
                _imPending = true;
            }
            else
            {
                _im.BackSpace();
            }
        }
        else
        {
            _re.BackSpace();
        }
        return GetNum();
    }

    // ── Очистка ────────────────────────────────────────────────────────────
    public override string Clear()
    {
        _re.Clear();
        _im.Clear();
        _editingIm = false;
        _imPending = false;
        return "0";
    }
}