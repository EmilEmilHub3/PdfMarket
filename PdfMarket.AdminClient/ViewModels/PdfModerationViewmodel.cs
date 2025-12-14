using System.Collections.ObjectModel;
using System.Windows;
using PdfMarket.AdminClient.Infrastructure;
using PdfMarket.AdminClient.Services;
using PdfMarket.Contracts.Admin;

namespace PdfMarket.AdminClient.ViewModels;

/// <summary>
/// ViewModel for moderating uploaded PDFs.
/// Allows admins to view and delete PDFs.
/// </summary>
public class PdfModerationViewModel : ViewModelBase
{
    private readonly AdminApiClient adminApi;

    public ObservableCollection<AdminPdfListItemDto> Pdfs { get; } = new();

    private AdminPdfListItemDto? selectedPdf;
    public AdminPdfListItemDto? SelectedPdf
    {
        get => selectedPdf;
        set
        {
            selectedPdf = value;
            OnPropertyChanged();
            DeleteSelectedCommand.RaiseCanExecuteChanged();
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
            DeleteSelectedCommand.RaiseCanExecuteChanged();
        }
    }

    private string? errorMessage;
    public string? ErrorMessage
    {
        get => errorMessage;
        set { errorMessage = value; OnPropertyChanged(); }
    }

    public RelayCommand RefreshCommand { get; }
    public RelayCommand DeleteSelectedCommand { get; }

    public PdfModerationViewModel(AdminApiClient adminApi)
    {
        this.adminApi = adminApi;

        RefreshCommand = new RelayCommand(
            async () => await LoadAsync(),
            () => !IsBusy);

        DeleteSelectedCommand = new RelayCommand(
            async () => await DeleteSelectedAsync(),
            () => !IsBusy && SelectedPdf != null);
    }

    /// <summary>
    /// Loads all PDFs from the admin API.
    /// </summary>
    public async Task LoadAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = null;

            Pdfs.Clear();
            var items = await adminApi.GetPdfsAsync();

            foreach (var p in items)
                Pdfs.Add(p);
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
    /// Deletes the currently selected PDF after confirmation.
    /// </summary>
    private async Task DeleteSelectedAsync()
    {
        if (SelectedPdf is null)
            return;

        var confirm = MessageBox.Show(
            $"Delete PDF:\n\n{SelectedPdf.Title}\n\nThis cannot be undone.",
            "Confirm delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (confirm != MessageBoxResult.Yes)
            return;

        try
        {
            IsBusy = true;
            await adminApi.DeletePdfAsync(SelectedPdf.Id);

            Pdfs.Remove(SelectedPdf);
            SelectedPdf = null;
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
