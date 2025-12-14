using System.Windows;
using System.Windows.Controls;
using PdfMarket.AdminClient.ViewModels;

namespace PdfMarket.AdminClient.Views;

/// <summary>
/// View for managing users in the admin client.
/// 
/// Contains minimal code-behind to handle PasswordBox input,
/// which cannot be bound directly in WPF.
/// </summary>
public partial class UsersView : UserControl
{
    public UsersView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Transfers password input from PasswordBox to the ViewModel.
    /// </summary>
    private void NewPasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is UsersViewModel vm && sender is PasswordBox pb)
        {
            vm.NewPassword = pb.Password;
        }
    }
}
