﻿namespace Xray.Models;

public sealed class XrayNodeModel : ReactiveObject
{
    private bool _isChecked;
    private string _remark = string.Empty;
    private XrayNodeStatus _status;

    [Reactive]
    public int Id { get; set; }
    /// <summary>
    /// 类型
    /// </summary>
    [Reactive]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 别名
    /// </summary>
    [Reactive]
    public string Alias { get; set; } = string.Empty;

    /// <summary>
    /// 地址
    /// </summary>
    [Reactive]
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// 端口
    /// </summary>
    [Reactive]
    public int Port { get; set; }

    /// <summary>
    /// 原Url
    /// </summary>
    [Reactive]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// 状态
    /// </summary>
    public XrayNodeStatus Status
    {
        get => _status;
        set
        {
            this.RaiseAndSetIfChanged(ref _status, value);
            this.RaisePropertyChanged(nameof(Default));
            this.RaisePropertyChanged(nameof(Error));
            this.RaisePropertyChanged(nameof(Success));
        }
    }

    public bool Default => Status == XrayNodeStatus.Default;
    public bool Error => Status == XrayNodeStatus.Error;
    public bool Success => Status == XrayNodeStatus.Success;

    /// <summary>
    /// 下载速度 b/s
    /// </summary>
    [Reactive]
    public double Speed { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark
    {
        get => _remark;
        set
        {
            this.RaiseAndSetIfChanged(ref _remark, value);
            this.RaisePropertyChanged(nameof(Tip));
        }
    }

    /// <summary>
    /// 是否使用
    /// </summary>
    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            this.RaiseAndSetIfChanged(ref _isChecked, value);
            this.RaisePropertyChanged(nameof(Tip));
        }
    }

    /// <summary>
    /// 更新时间
    /// </summary>
    [Reactive]
    public DateTime Updated { get; set; }

    public string Tip => ToString();
    public string Country => Constant.GetCountry(Alias);

    public new string ToString()
    {
        var stateText = Status switch
        {
            XrayNodeStatus.Default => "未检测",
            XrayNodeStatus.Error => "不可用",
            XrayNodeStatus.Success => "可用",
            _ => string.Empty
        };

        var useText = IsChecked ? "使用中" : "未使用";

        return $"""
            {useText}
            状态：{stateText}
            速度：{Remark}           
            名称：{Alias}
            协议：{Type}://{Host}:{Port}
            """;
    }
}


public static class Constant
{
    public static string GetCountry(string text)
    {
        var item = "未知";
        foreach (var country in Countries)
        {
            if (text.Contains(country))
            {
                item = country;
                break;

            }
        }
        return item;
    }
    private static string[] Countries =>
    [
        "阿富汗","阿尔巴尼亚","阿尔及利亚","安道尔","安哥拉","安提瓜和巴布达","阿根廷",
        "亚美尼亚","澳大利亚","奥地利","阿塞拜疆","巴哈马","巴林","孟加拉国","巴巴多斯",
        "白俄罗斯","比利时","伯利兹","贝宁","不丹","玻利维亚","波斯尼亚和黑塞哥维那",
        "博茨瓦纳","巴西","文莱","保加利亚","布基纳法索","布隆迪","佛得角","柬埔寨",
        "喀麦隆","加拿大","中非共和国","乍得","智利","中国","哥伦比亚","科摩罗","刚果民主共和国",
        "刚果共和国","哥斯达黎加","克罗地亚","古巴","塞浦路斯","捷克共和国","丹麦",
        "吉布提","多米尼加共和国","东帝汶","厄瓜多尔","埃及","萨尔瓦多","赤道几内亚",
        "厄立特里亚","爱沙尼亚","埃塞俄比亚","斐济","芬兰","法国","加蓬","冈比亚",
        "格鲁吉亚","德国","加纳","希腊","格林纳达","危地马拉","几内亚","几内亚比绍",
        "圭亚那","海地","洪都拉斯","匈牙利","冰岛","印度","印度尼西亚","伊朗","伊拉克",
        "爱尔兰","以色列","意大利","牙买加","日本","约旦","哈萨克斯坦","肯尼亚",
        "基里巴斯","科威特","吉尔吉斯斯坦","老挝","拉脱维亚","黎巴嫩","莱索托","利比里亚",
        "利比亚","列支敦士登","立陶宛","卢森堡","马其顿","马达加斯加","马拉维","马来西亚",
        "马尔代夫","马里","马耳他","马绍尔群岛","毛里塔尼亚","毛里求斯","墨西哥","密克罗尼西亚联邦",
        "摩尔多瓦","摩纳哥","蒙古","黑山共和国","摩洛哥","莫桑比克","缅甸","纳米比亚",
        "瑙鲁","尼泊尔","荷兰","新西兰","尼加拉瓜","尼日尔","尼日利亚","朝鲜","挪威",
        "阿曼","巴基斯坦","帕劳","巴拿马","巴布亚新几内亚","巴拉圭","秘鲁","菲律宾","波兰",
        "葡萄牙","卡塔尔","罗马尼亚","俄罗斯","卢旺达","圣基茨和尼维斯","圣卢西亚","圣文森特和格林纳丁斯",
        "萨摩亚","圣马力诺","圣多美和普林西比","沙特阿拉伯","塞内加尔","塞尔维亚","塞舌尔",
        "塞拉利昂","新加坡","斯洛伐克","斯洛文尼亚","所罗门群岛","索马里","南非","韩国",
        "南苏丹","西班牙","斯里兰卡","苏丹","苏里南","斯威士兰","瑞典","瑞士","叙利亚",
        "塔吉克斯坦","坦桑尼亚","泰国","东帝汶","多哥","汤加","特立尼达和多巴哥","突尼斯",
        "土耳其","土库曼斯坦","图瓦卢","乌干达","乌克兰","阿拉伯联合酋长国","英国","美国",
        "乌拉圭","乌兹别克斯坦","瓦努阿图","梵蒂冈","委内瑞拉","越南","也门","赞比亚","津巴布韦"
    ];
}