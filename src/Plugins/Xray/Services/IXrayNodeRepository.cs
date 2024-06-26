﻿namespace Xray.Services;

/// <summary>
/// Xray 网络节点仓储
/// </summary>
public interface IXrayNodeRepository
{
    List<XrayNodeEntity> GetList();
    bool DeleteById(int id);
    Task DeleteByStatusError();
    Task Update(XrayNodeEntity entity);
    void SaveNodes(List<XrayNodeEntity> entities);
    void SetChecked(XrayNodeEntity entity);
}
