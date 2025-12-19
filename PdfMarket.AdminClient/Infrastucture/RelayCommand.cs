using System;
using System.Windows.Input;

namespace PdfMarket.AdminClient.Infrastructure;

/// <summary>
/// Minimal ICommand implementation used in MVVM.
/// Supports parameterless execution and optional CanExecute logic.
/// </summary>
public sealed class RelayCommand : ICommand
{
    private readonly Action execute;
    private readonly Func<bool>? canExecute;

    /// <summary>
    /// Creates a new RelayCommand.
    /// </summary>
    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
        this.canExecute = canExecute;
    }

    public bool CanExecute(object? parameter)
        => canExecute?.Invoke() ?? true;

    public void Execute(object? parameter)
        => execute();

    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Notifies WPF that CanExecute state has changed.
    /// </summary>
    public void RaiseCanExecuteChanged()
        => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
