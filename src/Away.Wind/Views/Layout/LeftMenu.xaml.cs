﻿using System.Windows.Media.Animation;

namespace Away.Wind.Views;

public partial class LeftMenu : UserControl
{
    public LeftMenu()
    {
        InitializeComponent();
        this.Loaded += LeftMenu_Loaded;
    }

    private void LeftMenu_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not LeftMenuVM vm)
        {
            return;
        }

        vm.PropertyChanged += Vm_PropertyChanged;
        this.LV.SelectedValue = vm.DefaultUrl;
        vm.SetDefaultMenu();
    }

    private void Vm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not LeftMenuVM vm)
        {
            return;
        }
        if (e.PropertyName == nameof(LeftMenuVM.Toggle))
        {
            ToggleChange(vm.Toggle);
            return;
        }
    }

    /// <summary>
    /// 展开|收起菜单
    /// </summary>
    /// <param name="toggle"></param>
    private void ToggleChange(bool toggle)
    {
        if (toggle)
        {
            ((Storyboard)FindResource("ShowMenu")).Begin();
        }
        else
        {
            ((Storyboard)FindResource("HideMenu")).Begin();
        }
    }
}

