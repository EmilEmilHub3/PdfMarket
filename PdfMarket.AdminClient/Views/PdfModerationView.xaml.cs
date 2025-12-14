using System.Windows.Controls;

namespace PdfMarket.AdminClient.Views;

/// <summary>
/// View for PDF moderation.
/// 
/// Contains only UI logic; all behavior is handled
/// by PdfModerationViewModel via data binding.
/// </summary>
public partial class PdfModerationView : UserControl
{
    public PdfModerationView()
    {
        InitializeComponent();
    }
}
