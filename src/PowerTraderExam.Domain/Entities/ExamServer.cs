namespace PowerTraderExam.Domain.Entities;

/// <summary>
/// 考试服务器。每台物理或逻辑考试机对应一个 <see cref="ServerId"/>，与配置中的 API Key 绑定，用于考生同步与成绩回写鉴权。
/// </summary>
public class ExamServer
{
    /// <summary>主键。</summary>
    public long Id { get; set; }

    /// <summary>服务器唯一标识，与 <c>ApiKeys:Servers:{serverId}</c> 配置项对应。</summary>
    public string ServerId { get; set; } = string.Empty;

    /// <summary>服务器名称（展示用）。</summary>
    public string ServerName { get; set; } = string.Empty;

    /// <summary>是否启用；禁用后不应再发放该服务器的 API Key 业务访问（由鉴权层控制）。</summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>创建时间（UTC）。</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
