namespace Away.App.Views;

[View("xray-node")]
public partial class XrayNodesView : ReactiveUserControl<XrayNodesViewModel>, IView
{
    private readonly DataGrid _dataGrid;
    public XrayNodesView()
    {
        ViewModel = AwayLocator.GetViewModel<XrayNodesViewModel>();
        InitializeComponent();

        _dataGrid = this.FindControl<DataGrid>("DGXrayNode")!;
        _dataGrid.DoubleTapped += DataGrid_DoubleTapped;
        _dataGrid.LoadingRow += DataGrid_LoadingRow;
    }

    private void DataGrid_LoadingRow(object? sender, DataGridRowEventArgs e)
    {
        if (e.Row.DataContext is not XrayNodeModel model)
        {
            return;
        }
        e.Row.Background = model.IsChecked ? Brush.Parse("#ff9300") : null;
    }

    private void DataGrid_DoubleTapped(object? sender, TappedEventArgs e)
    {
        MessageBus.Current.Publish(MessageBusType.Event, e, "DGXrayNode");
    }
}