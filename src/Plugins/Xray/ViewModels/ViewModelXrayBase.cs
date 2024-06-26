﻿namespace Xray.ViewModels;

public abstract class ViewModelXrayBase : ViewModelBase
{
    protected readonly IXrayService _xrayService;
    protected readonly IXrayMapper _mapper;


    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public ViewModelXrayBase(IXrayService xrayService, IXrayMapper mapper)
    {
        _xrayService = xrayService;
        _mapper = mapper;

        SaveCommand = ReactiveCommand.Create(OnSaveCommand);
        CancelCommand = ReactiveCommand.Create(OnCancelCommand);
        Init();
    }


    protected abstract void Init();
    protected virtual void OnSaveCommand()
    {
        _xrayService.SaveConfig();
        MessageShow.Success("保存成功");
    }

    protected void OnCancelCommand()
    {
        _xrayService.GetConfig();
        Init();
    }

    protected override void OnActivation()
    {
        _xrayService.GetConfig();
        Init();
    }
}
