using Avalonia.Controls;
using Avalonia.Input;
using UniversalCaluclator.ViewModels;

namespace UniversalCaluclator.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        KeyDown += OnKeyDown;
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm) return;

        // Ctrl+C / Ctrl+V
        if (e.KeyModifiers == KeyModifiers.Control)
        {
            if (e.Key == Key.C) { vm.CopyCommand.Execute(null);  e.Handled = true; return; }
            if (e.Key == Key.V) { vm.PasteCommand.Execute(null); e.Handled = true; return; }
        }

        string? tag = e.Key switch
        {
            Key.D0 or Key.NumPad0       => "0",
            Key.D1 or Key.NumPad1       => "1",
            Key.D2 or Key.NumPad2       => "2",
            Key.D3 or Key.NumPad3       => "3",
            Key.D4 or Key.NumPad4       => "4",
            Key.D5 or Key.NumPad5       => "5",
            Key.D6 or Key.NumPad6       => "6",
            Key.D7 or Key.NumPad7       => "7",
            Key.D8 or Key.NumPad8       => "8",
            Key.D9 or Key.NumPad9       => "9",
            Key.Add    or Key.OemPlus   when e.KeyModifiers == KeyModifiers.None => "30",
            Key.Subtract or Key.OemMinus => "31",
            Key.Multiply                 => "32",
            Key.Divide   or Key.OemQuestion => "33",
            Key.Return   or Key.Enter    => "36",
            Key.Escape                   => "37",
            Key.Back                     => "20",
            Key.Delete                   => "21",
            Key.OemPeriod or Key.OemComma or Key.Decimal => "18",
            Key.I when e.KeyModifiers == KeyModifiers.None => "19",
            _ => null
        };

        if (tag != null)
        {
            vm.PressButtonCommand.Execute(tag);
            e.Handled = true;
        }
    }
}