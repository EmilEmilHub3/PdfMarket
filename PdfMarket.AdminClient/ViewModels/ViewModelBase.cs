using System.ComponentModel;

using System.Runtime.CompilerServices;

/// <summary>
/// Base class for all ViewModels in the Admin Client.
/// Implements INotifyPropertyChanged to support WPF data binding.
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the PropertyChanged event to notify the UI that
    /// a bound property value has changed.
    /// </summary>
    /// <param name="name">
    /// Name of the property that changed.
    /// Automatically provided by CallerMemberName.
    /// </param>
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
