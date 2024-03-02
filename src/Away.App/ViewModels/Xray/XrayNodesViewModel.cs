﻿using Avalonia.Input.Platform;
using System.Threading.Tasks;

namespace Away.App.ViewModels.Xray;

[ViewModel]
internal class XrayNodesViewModel : ViewModelBase
{
    private readonly IXrayNodeRepository _xrayNodeRepository;
    private readonly IXrayNodeService _xrayNodeService;
    private readonly IXrayNodeSubRepository _xrayNodeSubRepository;
    private readonly IXrayService _xrayService;
    private readonly IMapper _mapper;
    private readonly IClipboard _clipboard;

    public ICommand ResetCommand { get; }
    public ICommand UpdateNodeCommand { get; }
    public ICommand CheckedCommand { get; }
    public ICommand SpeedTest { get; }
    public ICommand CopyCommand { get; }
    public ICommand PasteCommand { get; }
    public ICommand DeleteCommand { get; }

    public XrayNodesViewModel(
        IXrayNodeRepository xrayNodeRepository,
        IXrayNodeService xrayNodeService,
        IXrayNodeSubRepository xrayNodeSubRepository,
        IXrayService xrayService,
        IMapper mapper,
        IClipboard clipboard
        )
    {
        _mapper = mapper;
        _xrayNodeRepository = xrayNodeRepository;
        _xrayNodeService = xrayNodeService;
        _xrayNodeSubRepository = xrayNodeSubRepository;
        _xrayService = xrayService;
        _clipboard = clipboard;

        ResetCommand = ReactiveCommand.Create(OnResetCommand);
        UpdateNodeCommand = ReactiveCommand.Create(OnUpdateNodeCommand);
        CheckedCommand = ReactiveCommand.Create(OnCheckedCommand);
        SpeedTest = ReactiveCommand.Create(OnSpeedTest);
        CopyCommand = ReactiveCommand.Create(OnCopyCommand);
        PasteCommand = ReactiveCommand.Create(OnPasteCommand);
        DeleteCommand = ReactiveCommand.Create(OnDeleteCommand);

        OnResetCommand();
        MessageBus.Current.Subscribe(MessageBusType.Event, o => OnCheckedCommand(), "DGXrayNode");
    }


    private bool _isEnableXray;
    /// <summary>
    /// 是否启用xray
    /// </summary>
    public bool IsEnableXray
    {
        get => _isEnableXray;
        set
        {
            this.RaiseAndSetIfChanged(ref _isEnableXray, value);
            if (_isEnableXray)
            {
                _xrayService.XrayStart();
                var inbound = _xrayService.Config.inbounds.FirstOrDefault();
                if (inbound == null)
                {
                    return;
                }
            }
            else
            {
                _xrayService.XrayClose();
            }
        }
    }

    private bool _isEnableGlobalProxy;
    /// <summary>
    /// 是否启用全局网络代理
    /// </summary>
    public bool IsEnableGlobalProxy
    {
        get => _isEnableGlobalProxy;
        set
        {
            this.RaiseAndSetIfChanged(ref _isEnableGlobalProxy, value);
            if (_isEnableGlobalProxy)
            {
                _xrayService.OpenGlobalProxy();
            }
            else
            {
                _xrayService.CloseGlobalProxy();
            }
        }
    }

    /// <summary>
    /// 节点列表
    /// </summary>
    [Reactive]
    public ObservableCollection<XrayNodeModel> XrayNodeItemsSource { get; set; } = [];

    [Reactive]
    public XrayNodeModel? XrayNodeSelectedItem { get; set; }

    /// <summary>
    /// 刷新
    /// </summary>
    private void OnResetCommand()
    {
        IsEnableXray = _xrayService.IsEnable;
        IsEnableGlobalProxy = _xrayService.IsEnableGlobalProxy;
        var xraynodes = _xrayNodeRepository.GetList().OrderByDescending(o => o.Status);
        var items = xraynodes.Select(_mapper.Map<XrayNodeModel>);
        XrayNodeItemsSource = new ObservableCollection<XrayNodeModel>(items);
    }
    /// <summary>
    /// 更新订阅
    /// </summary>
    private async void OnUpdateNodeCommand()
    {
        await _xrayNodeService.SetXrayNodeByUrl("https://proxy.v2gh.com/https://raw.githubusercontent.com/Pawdroid/Free-servers/main/sub");
        //await _xrayNodeService.SetXrayNodeByUrl("https://bulinkbulink.com/freefq/free/master/v2");

        var subs = _xrayNodeSubRepository.GetListByEnable();
        foreach (var sub in subs)
        {
            Show($"正在更新订阅{sub.Remark}...");
            await _xrayNodeService.SetXrayNodeByUrl(sub.Url);
        }
        OnResetCommand();
        Show("节点订阅更新完成");
    }
    /// <summary>
    /// 使用节点
    /// </summary>
    private void OnCheckedCommand()
    {
        var model = XrayNodeSelectedItem;
        if (model == null || string.IsNullOrWhiteSpace(model.Url))
        {
            return;
        }

        var entity = _mapper.Map<XrayNodeEntity>(model);
        _xrayService.Config.SetOutbound(entity);
        _xrayService.SaveConfig();
        _xrayService.XrayRestart();
        IsEnableXray = _xrayService.IsEnable;

        // 设置选中节点
        foreach (var item in XrayNodeItemsSource)
        {
            item.IsChecked = item.Url == model.Url;
        }
        var items = XrayNodeItemsSource.Select(_mapper.Map<XrayNodeEntity>).ToList();
        _xrayNodeRepository.SaveNodes(items);
        Show("切换节点成功");
        OnResetCommand();
    }
    /// <summary>
    /// 测试所有节点速度
    /// </summary>
    private void OnSpeedTest()
    {
        Show("开始测试");
        var items = XrayNodeItemsSource.Select(_mapper.Map<XrayNodeEntity>).ToList();
        var speedService = new XrayNodeSpeedTest(items, 10, 3000);
        speedService.OnTesting += (entity) =>
        {
            var model = XrayNodeItemsSource.FirstOrDefault(o => o.Url == entity.Url);
            if (model == null)
            {
                return;
            }
            model.Status = XrayNodeStatus.Default;
            model.Remark = "检测中...";
        };

        speedService.OnTested += (e) =>
        {
            var result = e.Data;
            var entity = e.XrayNode;
            var model = XrayNodeItemsSource.FirstOrDefault(o => o.Url == entity.Url)!;

            if (result.IsSuccess)
            {
                model.Status = entity.Status = XrayNodeStatus.Success;
                model.Remark = entity.Remark = result.Speed;
                model.Updated = DateTime.Now;
            }
            else
            {
                model.Status = entity.Status = XrayNodeStatus.Error;
                model.Remark = entity.Remark = result.Error;
            }
            _xrayNodeRepository.Update(entity);

        };

        speedService.OnCompeleted += async () =>
        {
            _xrayNodeRepository.DeleteByStatusError();
            OnResetCommand();
            await Task.CompletedTask;
        };
        speedService.Listen();
    }
    /// <summary>
    /// 复制节点
    /// </summary>
    private async void OnCopyCommand()
    {
        var model = XrayNodeSelectedItem;
        if (model == null)
        {
            return;
        }

        if (_clipboard != null)
        {
            await _clipboard.SetTextAsync(model.Url);
        }
    }
    /// <summary>
    /// 粘贴节点
    /// </summary>
    private async void OnPasteCommand()
    {
        var text = string.Empty;
        if (_clipboard != null)
        {
            text = await _clipboard.GetTextAsync() ?? string.Empty;
        }
        var items = text.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        foreach (var item in items)
        {
            var base64Pattern = "^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)$";
            if (System.Text.RegularExpressions.Regex.IsMatch(item, base64Pattern))
            {
                _xrayNodeService.SetXrayNodeByBase64String(item);
            }
            else
            {
                _xrayNodeService.SaveXrayNodeByList([item]);
            }
        }
        OnResetCommand();
    }
    /// <summary>
    /// 删除节点
    /// </summary>
    private void OnDeleteCommand()
    {
        if (XrayNodeSelectedItem == null)
        {
            return;
        }
        _xrayNodeRepository.DeleteByUrl(XrayNodeSelectedItem.Url);
        XrayNodeItemsSource.Remove(XrayNodeSelectedItem);
    }

    private static void Show(string message, NotificationType notificationType = NotificationType.Information)
    {
        Log.Information(message);
        MessageBus.Current.Nofity(string.Empty, message, notificationType);
    }

}