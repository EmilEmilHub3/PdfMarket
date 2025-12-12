using System.Windows;
using System.Windows.Controls;
using PdfMarket.AdminClient.ViewModels;

namespace PdfMarket.AdminClient.Views;

public partial class UsersView : UserControl
{
    public UsersView()
    {
        InitializeComponent();
    }

    private void NewPasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is UsersViewModel vm && sender is PasswordBox pb)
            vm.NewPassword = pb.Password;
    }
}
