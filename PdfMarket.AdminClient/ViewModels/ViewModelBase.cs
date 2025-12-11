using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PdfMarket.AdminClient.ViewModels;

// Base for all ViewModels (INotifyPropertyChanged)
public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
