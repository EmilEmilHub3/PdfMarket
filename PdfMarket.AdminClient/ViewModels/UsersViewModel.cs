using System;
using System.Collections.ObjectModel;
using System.Windows;
using PdfMarket.AdminClient.Infrastructure;
using PdfMarket.AdminClient.Services;
using PdfMarket.Contracts.Admin;

namespace PdfMarket.AdminClient.ViewModels;

/// <summary>
/// ViewModel for administering users.
/// </summary>
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
            ResetPasswordCommand.RaiseCanExecuteChanged();
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

    private string? newPassword;
    public string? NewPassword
    {
        get => newPassword;
        set
        {
            newPassword = value;
            OnPropertyChanged();
            ResetPasswordCommand.RaiseCanExecuteChanged();
        }
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
            ResetPasswordCommand.RaiseCanExecuteChanged();
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
    public RelayCommand ResetPasswordCommand { get; }

    /// <summary>
    /// Initializes the user administration ViewModel.
    /// </summary>
    /// <param name="adminApi">Authenticated admin API client.</param>
    public UsersViewModel(AdminApiClient adminApi)
    {
        this.adminApi = adminApi;

        RefreshCommand = new RelayCommand(async () => await LoadAsync(), () => !IsBusy);
        SaveCommand = new RelayCommand(async () => await SaveAsync(), () => CanSave());
        ResetPasswordCommand = new RelayCommand(async () => await ResetPasswordAsync(), () => CanResetPassword());
    }

    /// <summary>
    /// Loads all users from the admin API.
    /// </summary>
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

    /// <summary>
    /// Copies selected user data into the edit fields.
    /// </summary>
    private void LoadSelectedIntoEditor()
    {
        if (SelectedUser is null)
        {
            EditEmail = null;
            EditPoints = null;
            NewPassword = null;
            return;
        }

        EditEmail = SelectedUser.Email;
        EditPoints = SelectedUser.PointsBalance;
        NewPassword = null;
    }

    /// <summary>
    /// Determines whether the selected user can be saved.
    /// </summary>
    private bool CanSave()
    {
        if (IsBusy || SelectedUser is null)
            return false;

        return EditEmail != SelectedUser.Email ||
               EditPoints != SelectedUser.PointsBalance;
    }

    /// <summary>
    /// Saves edited user data to the backend.
    /// </summary>
    private async Task SaveAsync()
    {
        if (SelectedUser is null)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            var req = new UpdateUserRequest(
                Email: EditEmail,
                PointsBalance: EditPoints
            );

            await adminApi.UpdateUserAsync(SelectedUser.Id, req);

            var index = Users.IndexOf(SelectedUser);
            if (index >= 0)
            {
                var updated = SelectedUser with
                {
                    Email = EditEmail ?? SelectedUser.Email,
                    PointsBalance = EditPoints ?? SelectedUser.PointsBalance
                };

                Users[index] = updated;
                SelectedUser = updated;
            }

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

    /// <summary>
    /// Determines whether the password can be reset.
    /// </summary>
    private bool CanResetPassword()
    {
        if (IsBusy || SelectedUser is null)
            return false;

        return !string.IsNullOrWhiteSpace(NewPassword);
    }

    /// <summary>
    /// Resets the password of the selected user after confirmation.
    /// </summary>
    private async Task ResetPasswordAsync()
    {
        if (SelectedUser is null || string.IsNullOrWhiteSpace(NewPassword))
            return;

        var confirm = MessageBox.Show(
            $"Reset password for:\n\n{SelectedUser.UserName}\n\nContinue?",
            "Confirm password reset",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (confirm != MessageBoxResult.Yes)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            var ok = await adminApi.ResetPasswordAsync(
                SelectedUser.Id,
                new ResetPasswordRequest(NewPassword));

            if (!ok)
            {
                ErrorMessage = "Reset password failed.";
                return;
            }

            MessageBox.Show("Password reset", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);

            NewPassword = null;
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
