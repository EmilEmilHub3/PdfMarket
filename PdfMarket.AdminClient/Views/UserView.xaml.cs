using System.Windows;
using System.Windows.Controls;
using PdfMarket.AdminClient.ViewModels;

namespace PdfMarket.AdminClient.Views;

/// <summary>
/// View for managing users in the admin client.
///
/// Contains minimal code-behind logic required for handling
/// PasswordBox input, which cannot be bound directly in WPF.
/// </summary>
public partial class UsersView : UserControl
{
    /// <summary>
    /// Initializes the users management view.
    /// </summary>
    public UsersView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Transfers the entered password from the PasswordBox
    /// to the UsersViewModel.
    ///
    /// This approach is required because WPF does not allow
    /// direct binding to the PasswordBox.Password property.
    /// </summary>
    private void NewPasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is UsersViewModel vm && sender is PasswordBox pb)
        {
            vm.NewPassword = pb.Password;
        }
    }
}
