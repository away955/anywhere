﻿using System.Windows.Controls;
using System.Windows;

namespace Window2.Utilities;

public class Btn : RadioButton
{
    static Btn()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Btn), new FrameworkPropertyMetadata(typeof(Btn)));
    }
}