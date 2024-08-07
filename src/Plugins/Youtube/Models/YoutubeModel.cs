﻿namespace Youtube.Models;

public sealed class YoutubeModel : ReactiveObject
{
    private YoutubeVideoState _state;

    /// <summary>
    /// 删除视频
    /// </summary>
    public ICommand DelCommand { get; set; } = null!;
    /// <summary>
    /// 下载视频
    /// </summary>
    public ICommand DownloadCommand { get; set; } = null!;
    /// <summary>
    /// 取消下载
    /// </summary>
    public ICommand CancelCommand { get; set; } = null!;
    /// <summary>
    /// 打开文件夹
    /// </summary>
    public ICommand OpenFolderCommand { get; set; } = null!;
    /// <summary>
    /// 详情
    /// </summary>
    public ICommand InfoCommand { get; set; } = null!;


    [Reactive]
    public int Id { get; set; }
    [Reactive]
    public string Title { get; set; } = string.Empty;
    [Reactive]
    public string Description { get; set; } = string.Empty;
    [Reactive]
    public string Source { get; set; } = string.Empty;
    /// <summary>
    /// 视频地址
    /// </summary>
    [Reactive]
    public string VideoPath { get; set; } = string.Empty;
    [Reactive]
    public string VideoArea { get; set; } = string.Empty;
    /// <summary>
    /// 图片地址
    /// </summary>
    [Reactive]
    public string ImagePath { get; set; } = string.Empty;
    [Reactive]
    public string Author { get; set; } = string.Empty;
    [Reactive]
    public DateTime? Uploaded { get; set; }
    public YoutubeVideoState State
    {
        get => _state;
        set
        {
            this.RaiseAndSetIfChanged(ref _state, value);
            this.RaisePropertyChanged(nameof(IsVisibleProgressBar));
            this.RaisePropertyChanged(nameof(IsVisibleDownloadBtn));
            this.RaisePropertyChanged(nameof(IsVisibleFolderBtn));
        }
    }
    [Reactive]
    public DateTime Created { get; set; }
    [Reactive]
    public DateTime Updated { get; set; }

    [Reactive]
    public double ProgressBarValue { get; set; }
    public bool IsVisibleProgressBar => State == YoutubeVideoState.Downloading;
    public bool IsVisibleDownloadBtn => State == YoutubeVideoState.Waiting || State == YoutubeVideoState.Error;
    public bool IsVisibleFolderBtn => State != YoutubeVideoState.Downloading;

    public string TitileShort => TextLength(Title) > 30 ? $"{TextSpilt(Title, 30)}..." : Title;

    private static int TextLength(string text)
    {
        return Encoding.Default.GetBytes(text).Length;
    }
    private static string TextSpilt(string text, int len)
    {
        var bytes = Encoding.Default.GetBytes(text);
        return Encoding.Default.GetString(bytes[..len]).Replace("�", string.Empty);
    }
}
