using System.Collections.ObjectModel;
using System.Windows;
using PdfMarket.AdminClient.Infrastructure;
using PdfMarket.AdminClient.Services;
using PdfMarket.Contracts.Admin;

namespace PdfMarket.AdminClient.ViewModels;

public class UsersViewModel : ViewModelBase
{
    private readonly AdminApiClient adminApi;

    public ObservableCollection<UserSummaryDto> Users { get; } = new();

    private UserSummaryDto? selectedUser;
    public UserSummaryDto? SelectedUser
    {
        get => selectedUser;
        set
        {
            selectedUser = value;
            OnPropertyChanged();
            LoadSelectedIntoEditor();
            SaveCommand.RaiseCanExecuteChanged();
        }
    }

    private string? editEmail;
    public string? EditEmail
    {
        get => editEmail;
        set { editEmail = value; OnPropertyChanged(); SaveCommand.RaiseCanExecuteChanged(); }
    }

    private int? editPoints;
    public int? EditPoints
    {
        get => editPoints;
        set { editPoints = value; OnPropertyChanged(); SaveCommand.RaiseCanExecuteChanged(); }
    }

    private bool isBusy;
    public bool IsBusy
    {
        get => isBusy;
        set
        {
            isBusy = value;
            OnPropertyChanged();
            RefreshCommand.RaiseCanExecuteChanged();
            SaveCommand.RaiseCanExecuteChanged();
        }
    }

    private string? errorMessage;
    public string? ErrorMessage
    {
        get => errorMessage;
        set { errorMessage = value; OnPropertyChanged(); }
    }

    public RelayCommand RefreshCommand { get; }
    public RelayCommand SaveCommand { get; }

    public UsersViewModel(AdminApiClient adminApi)
    {
        this.adminApi = adminApi;

        RefreshCommand = new RelayCommand(
            async () => await LoadAsync(),
            () => !IsBusy);

        SaveCommand = new RelayCommand(
            async () => await SaveAsync(),
            () => CanSave());
    }

    public async Task LoadAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = null;

            Users.Clear();
            var items = await adminApi.GetUsersAsync();
            foreach (var u in items)
                Users.Add(u);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void LoadSelectedIntoEditor()
    {
        if (SelectedUser is null)
        {
            EditEmail = null;
            EditPoints = null;
            return;
        }

        EditEmail = SelectedUser.Email;
        EditPoints = SelectedUser.PointsBalance;
    }

    private bool CanSave()
    {
        if (IsBusy || SelectedUser is null)
            return false;

        return EditEmail != SelectedUser.Email ||
               EditPoints != SelectedUser.PointsBalance;
    }

    private async Task SaveAsync()
    {
        if (SelectedUser is null)
            return;

        try
        {
            IsBusy = true;

            var req = new UpdateUserRequest(
                Email: EditEmail,
                PointsBalance: EditPoints
            );

            await adminApi.UpdateUserAsync(SelectedUser.Id, req);

            MessageBox.Show("User updated", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
