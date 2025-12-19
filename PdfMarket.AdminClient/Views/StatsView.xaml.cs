using System.Windows.Controls;

namespace PdfMarket.AdminClient.Views;

/// <summary>
/// View for displaying platform statistics.
///
/// Fully driven by data binding to StatsViewModel.
/// Contains no code-behind logic beyond initialization.
/// </summary>
public partial class StatsView : UserControl
{
    /// <summary>
    /// Initializes the statistics view.
    /// </summary>
    public StatsView()
    {
        InitializeComponent();
    }
}
