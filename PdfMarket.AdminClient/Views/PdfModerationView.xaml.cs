using System.Windows.Controls;

namespace PdfMarket.AdminClient.Views;

/// <summary>
/// View responsible for displaying and moderating PDFs.
///
/// Contains no business logic; all behavior is handled
/// by PdfModerationViewModel via data binding.
/// </summary>
public partial class PdfModerationView : UserControl
{
    /// <summary>
    /// Initializes the PDF moderation view.
    /// </summary>
    public PdfModerationView()
    {
        InitializeComponent();
    }
}
