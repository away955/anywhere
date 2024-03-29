﻿using System.Reactive.Disposables;

namespace Away.App.ViewModels;

public abstract class ViewModelBase : ReactiveObject, IActivatableViewModel
{
    public ViewModelActivator Activator { get; }

    public ViewModelBase()
    {
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables =>
        {
            OnActivation();
            Disposable.Create(OnDeactivation).DisposeWith(disposables);
        });
    }

    protected virtual void OnActivation()
    {

    }

    protected virtual void OnDeactivation()
    {

    }
}
