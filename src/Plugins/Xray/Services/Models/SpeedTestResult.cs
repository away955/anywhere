﻿namespace Xray.Services.Models;

public sealed class SpeedTestResult
{
    public required XrayNodeEntity Entity { get; set; }
    public bool IsSuccess { get; set; }
    public string Remark { get; set; } = string.Empty;
    public double Speed { get; set; }
    public string Error { get; set; } = string.Empty;
}