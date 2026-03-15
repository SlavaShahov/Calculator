using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UniversalCaluclator.Models.Controller;
using UniversalCaluclator.Models.Editors;
using UniversalCaluclator.Models.Numbers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Layout;

namespace UniversalCaluclator.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    // ── Контроллер ──────────────────────────────────────────────────────────
    private readonly TCtrl _ctrl;
    private int _funcParam = 2;
    private bool _complexFormat = true;   // true = a+bi, false = a (если im==0)

    // ── Привязки дисплея ────────────────────────────────────────────────────
    [ObservableProperty] private string _display        = "0";
    [ObservableProperty] private string _expressionLine = "";
    [ObservableProperty] private string _memoryIndicator = "";
    [ObservableProperty] private string _formatLabel    = "Комплексный";

    // ── Параметр n (степень/корень) — управляется кнопками +/− ────────────
    [ObservableProperty] private string _functionParamText = "2";

    [RelayCommand]
    private void ParamIncr()
    {
        if (int.TryParse(FunctionParamText, out int n) && n < 99)
        {
            _funcParam = n + 1;
            FunctionParamText = _funcParam.ToString();
        }
    }

    [RelayCommand]
    private void ParamDecr()
    {
        if (int.TryParse(FunctionParamText, out int n) && n > 2)
        {
            _funcParam = n - 1;
            FunctionParamText = _funcParam.ToString();
        }
    }
    [ObservableProperty] private string _resultPwr    = "";
    [ObservableProperty] private string _resultRoot   = "";
    [ObservableProperty] private string _resultMdl    = "";
    [ObservableProperty] private string _resultArgDeg = "";
    [ObservableProperty] private string _resultArgRad = "";
    [ObservableProperty] private ObservableCollection<string> _allRoots = new();

    // ── Отслеживание выражения для строки над дисплеем ─────────────────────
    private string _exprAccum  = "";   // накопленная левая часть
    private string _pendingOp  = "";   // символ ожидаемой операции

    public MainWindowViewModel()
    {
        _ctrl = new TCtrl(CalcMode.Complex, 10);
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  Главная команда кнопок
    // ═══════════════════════════════════════════════════════════════════════
    [RelayCommand]
    private void PressButton(string tag)
    {
        if (!int.TryParse(tag, out int cmd)) return;

        // Обновляем строку выражения до выполнения команды
        UpdateExprLine(cmd);

        string result = _ctrl.ExecCmd(cmd, out string mem);
        Display        = ApplyFormat(result);
        MemoryIndicator = mem;

        // При вводе нового числа — сбрасываем поля функций
        if (cmd is >= 0 and <= 19)
            ClearFuncResults();

        // При C — сбрасываем строку выражения
        if (cmd == CalcCmd.ClearAll)
        {
            _exprAccum     = "";
            _pendingOp     = "";
            ExpressionLine = "";
            ClearFuncResults();
        }
    }

    // ── Вспомогательные строки выражения ────────────────────────────────────
    private void UpdateExprLine(int cmd)
    {
        switch (cmd)
        {
            case CalcCmd.OpAdd: SetOp("+"); break;
            case CalcCmd.OpSub: SetOp("−"); break;
            case CalcCmd.OpMul: SetOp("×"); break;
            case CalcCmd.OpDvd: SetOp("÷"); break;
            case CalcCmd.FnSqr:
            {
                var (_, im) = CurrentComplex();
                string op = im != 0 ? $"({Display})²" : $"{Display}²";
                ExpressionLine = $"{op} =";
                _exprAccum = ""; _pendingOp = "";
                break;
            }
            case CalcCmd.FnRev:
            {
                var (_, im) = CurrentComplex();
                string op = im != 0 ? $"1/({Display})" : $"1/{Display}";
                ExpressionLine = $"{op} =";
                _exprAccum = ""; _pendingOp = "";
                break;
            }
            case CalcCmd.Equal:
                if (!string.IsNullOrEmpty(_pendingOp))
                    ExpressionLine = $"{_exprAccum} {_pendingOp} {Display} =";
                _exprAccum = "";
                _pendingOp = "";
                break;
            case CalcCmd.ClearAll:
                ExpressionLine = "";
                _exprAccum = "";
                _pendingOp = "";
                break;
        }
    }

    private void SetOp(string opSym)
    {
        if (string.IsNullOrEmpty(_exprAccum))
            _exprAccum = Display;
        ExpressionLine = $"{_exprAccum} {opSym}";
        _pendingOp     = opSym;
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  Специальные функции комплексных чисел
    // ═══════════════════════════════════════════════════════════════════════

    [RelayCommand]
    private void Power()
    {
        var (re, im) = CurrentComplex();
        string operand    = ApplyFormat(Display);
        string operandStr = im != 0 ? $"({operand})" : operand;

        var zn = new TComp(re, im).Power(_funcParam);
        ResultPwr      = ApplyFormat(zn.NumberString);
        Display        = ResultPwr;
        ExpressionLine = $"{operandStr}^{_funcParam} =";
        AllRoots.Clear();
    }

    [RelayCommand]
    private void Root()
    {
        var (re, im) = CurrentComplex();
        string operand    = ApplyFormat(Display);
        string operandStr = im != 0 ? $"({operand})" : operand;

        var roots = new TComp(re, im).Roots(_funcParam);
        AllRoots.Clear();
        foreach (var r in roots)
            AllRoots.Add(ApplyFormat(r.NumberString));

        ResultRoot     = AllRoots.Count > 0 ? AllRoots[0] : "";
        Display        = ResultRoot;
        ExpressionLine = $"{_funcParam}√{operandStr} =";
    }

    [RelayCommand]
    private void Modulus()
    {
        var (re, im) = CurrentComplex();
        var z        = new TComp(re, im);
        ResultMdl      = z.Modulus.ToString("G6", CultureInfo.InvariantCulture);
        Display        = ResultMdl;
        ExpressionLine = $"|{FormatComplex(re, im)}| =";
        AllRoots.Clear();
    }

    [RelayCommand]
    private void ArgDeg()
    {
        var (re, im) = CurrentComplex();
        var z        = new TComp(re, im);
        ResultArgDeg   = z.ArgumentDeg.ToString("G6", CultureInfo.InvariantCulture);
        Display        = ResultArgDeg;
        ExpressionLine = $"arg°({FormatComplex(re, im)}) =";
        AllRoots.Clear();
    }

    [RelayCommand]
    private void ArgRad()
    {
        var (re, im) = CurrentComplex();
        var z        = new TComp(re, im);
        ResultArgRad   = z.ArgumentRad.ToString("G6", CultureInfo.InvariantCulture);
        Display        = ResultArgRad;
        ExpressionLine = $"arg rad({FormatComplex(re, im)}) =";
        AllRoots.Clear();
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  Настройки формата
    // ═══════════════════════════════════════════════════════════════════════

    [RelayCommand]
    private void SetComplexFormat()
    {
        _complexFormat = true;
        FormatLabel    = "Комплексный";
        Display        = ApplyFormat(Display);
    }

    [RelayCommand]
    private void SetRealFormat()
    {
        _complexFormat = false;
        FormatLabel    = "Действительный";
        Display        = ApplyFormat(Display);
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  Буфер обмена
    // ═══════════════════════════════════════════════════════════════════════

    [RelayCommand]
    private async void Copy()
    {
        try
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime d)
            {
                var tl = TopLevel.GetTopLevel(d.MainWindow);
                if (tl?.Clipboard != null)
                    await tl.Clipboard.SetTextAsync(Display);
            }
        }
        catch { /* игнорируем */ }
    }

    [RelayCommand]
    private async void Paste()
    {
        try
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime d)
            {
                var tl = TopLevel.GetTopLevel(d.MainWindow);
                if (tl?.Clipboard == null) return;

                string? text = await tl.Clipboard.GetTextAsync();
                if (string.IsNullOrWhiteSpace(text)) return;

                text = text.Trim().Replace(",", ".");

                // Парсим как комплексное число
                var (re, im) = ParseComplexString(text);
                var z = new TComp(re, im);

                // Вставляем как новый операнд — не сбрасываем состояние контроллера,
                // чтобы не потерять накопленную операцию (например 3+4i + [вставить 2])
                _ctrl.SetEditorNumber(z.NumberString);
                Display = ApplyFormat(z.NumberString);
                ClearFuncResults();
            }
        }
        catch { /* игнорируем */ }
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  Справка
    // ═══════════════════════════════════════════════════════════════════════

    [RelayCommand]
    private void ShowHelp()
    {
        ShowMessageBox("О программе",
            "Калькулятор комплексных чисел\n" +
            "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n" +
            "ВВОД ЧИСЛА a+bi:\n" +
            "  Введите действительную часть,\n" +
            "  нажмите [i], введите мнимую часть.\n" +
            "  Пример: 3 [i] 4  →  3+4i\n\n" +
            "ОПЕРАЦИИ: +  −  ×  ÷  =  x²  1/x\n\n" +
            "СПЕЦИАЛЬНЫЕ ФУНКЦИИ (результат\n" +
            "в отдельных полях, не в выражении):\n" +
            "  zⁿ      — степень n (Pwr)\n" +
            "  ⁿ√z     — корень n-й степени (Root)\n" +
            "  |z|     — модуль (Mdl)\n" +
            "  arg °   — аргумент в градусах (Cnr)\n" +
            "  arg rad — аргумент в радианах (Cnr)\n\n" +
            "ПАМЯТЬ:\n" +
            "  MC — очистить    MS — сохранить\n" +
            "  MR — вставить    M+ — добавить\n\n" +
            "БУФЕР ОБМЕНА:\n" +
            "  Правка → Копировать / Вставить\n" +
            "  Ctrl+C / Ctrl+V\n\n" +
            "НАСТРОЙКА ФОРМАТА (меню Настройка):\n" +
            "  Комплексный    — всегда a+bi\n" +
            "  Действительный — a, если b=0\n\n" +
            "ГОРЯЧИЕ КЛАВИШИ:\n" +
            "  Enter / Return — =\n" +
            "  Esc            — C (сброс)\n" +
            "  Backspace      — ⌫\n" +
            "  Delete         — CE\n" +
            "  Ctrl+C / Ctrl+V — буфер обмена");
    }

    // ═══════════════════════════════════════════════════════════════════════
    //  Вспомогательные методы
    // ═══════════════════════════════════════════════════════════════════════

    private void ClearFuncResults()
    {
        ResultPwr    = "";
        ResultRoot   = "";
        ResultMdl    = "";
        ResultArgDeg = "";
        ResultArgRad = "";
        AllRoots.Clear();
    }

    /// <summary>
    /// Применить формат к строке числа (комплексный / действительный).
    /// </summary>
    private string ApplyFormat(string s)
    {
        var (re, im) = ParseComplexString(s);

        if (_complexFormat)
        {
            string reStr = re.ToString("G6", CultureInfo.InvariantCulture);
            string sign  = im >= 0 ? "+" : "";
            string imStr = im.ToString("G6", CultureInfo.InvariantCulture);
            return $"{reStr}{sign}{imStr}i";
        }
        else
        {
            if (Math.Abs(im) < 1e-12)
                return re.ToString("G6", CultureInfo.InvariantCulture);
            return s;
        }
    }

    /// <summary>
    /// Парсит строку комплексного числа на действительную и мнимую части
    /// </summary>
    private (double re, double im) ParseComplexString(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return (0, 0);
            
        s = s.Trim();
        int iIdx = s.LastIndexOf('i');
        
        if (iIdx < 0)
        {
            // Нет мнимой части - это действительное число
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out double re))
                return (re, 0);
            return (0, 0);
        }
        
        // Есть мнимая часть
        string imPart = s[..iIdx].Trim();
        
        // Ищем последний знак + или - для разделения действительной и мнимой частей
        int sep = -1;
        for (int k = imPart.Length - 1; k > 0; k--)
        {
            if (imPart[k] == '+' || imPart[k] == '-')
            {
                sep = k;
                break;
            }
        }
        
        double real = 0, im = 0;
        
        if (sep >= 0)
        {
            // Действительная часть
            string reStr = imPart[..sep].Trim();
            if (!string.IsNullOrEmpty(reStr))
                double.TryParse(reStr, NumberStyles.Any, CultureInfo.InvariantCulture, out real);
            
            // Мнимая часть
            string imStr = imPart[sep..].Trim();
            ParseImaginaryPart(imStr, out im);
        }
        else
        {
            // Только мнимая часть
            ParseImaginaryPart(imPart, out im);
        }
        
        return (real, im);
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

    /// <summary>
    /// Получить текущее комплексное число из Display.
    /// </summary>
    private (double re, double im) CurrentComplex()
    {
        return ParseComplexString(Display);
    }

    private static string FormatComplex(double re, double im)
    {
        string r = re.ToString("G6", CultureInfo.InvariantCulture);
        
        if (Math.Abs(im) < 1e-12)
            return r;
            
        string imStr;
        if (Math.Abs(Math.Abs(im) - 1) < 1e-12)
        {
            // Мнимая часть равна ±1
            imStr = im > 0 ? "+i" : "-i";
        }
        else
        {
            string imAbs = Math.Abs(im).ToString("G6", CultureInfo.InvariantCulture);
            imStr = im > 0 ? $"+{imAbs}i" : $"-{imAbs}i";
        }
        
        // Если действительная часть равна 0 и это не "0+..."
        if (Math.Abs(re) < 1e-12 && imStr.StartsWith("+"))
            return imStr.TrimStart('+');
        if (Math.Abs(re) < 1e-12 && imStr.StartsWith("-"))
            return imStr;
            
        return r + imStr;
    }

    private static void ShowMessageBox(string title, string message)
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime d) return;

        var closeBtn = new Button
        {
            Content = "Закрыть",
            HorizontalAlignment = HorizontalAlignment.Center,
            Width = 110, Height = 32,
            Background = new SolidColorBrush(Color.Parse("#1E66F5")),
            Foreground = Brushes.White,
            FontSize = 13
        };

        var grid = new Grid { RowDefinitions = new RowDefinitions("*,Auto") };
        var scroll = new ScrollViewer
        {
            Content = new TextBlock
            {
                Text = message,
                Margin = new Thickness(16, 12),
                TextWrapping = TextWrapping.Wrap,
                Foreground = new SolidColorBrush(Color.Parse("#CDD6F4")),
                FontFamily = new FontFamily("Consolas, Courier New"),
                FontSize = 12
            }
        };
        var btnRow = new StackPanel
        {
            Margin = new Thickness(0, 6, 0, 10),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        btnRow.Children.Add(closeBtn);
        Grid.SetRow(scroll, 0);
        Grid.SetRow(btnRow, 1);
        grid.Children.Add(scroll);
        grid.Children.Add(btnRow);

        var win = new Window
        {
            Title = title,
            Width = 420, Height = 480,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Background = new SolidColorBrush(Color.Parse("#1E1E2E")),
            Content = grid
        };
        closeBtn.Click += (_, _) => win.Close();
        win.ShowDialog(d.MainWindow!);
    }
}