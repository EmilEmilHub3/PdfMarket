using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PdfMarket.AdminClient.ViewModels;

/// <summary>
/// Base class for all ViewModels.
/// Implements INotifyPropertyChanged to support WPF data binding.
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Notifies the UI that a property value has changed.
    /// CallerMemberName automatically supplies the property name.
    /// </summary>
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
