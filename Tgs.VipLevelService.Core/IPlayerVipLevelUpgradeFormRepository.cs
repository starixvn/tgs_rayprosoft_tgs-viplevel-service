using System.Text.Json.Nodes;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService;

public interface IPlayerVipLevelUpgradeFormRepository
{
	ValueTask CreateAsync(Guid id, Guid brandId, string playerCode, int vipLevel, decimal amount, PlayerVipLevelUpgradeFormSnapshotVipLevelInfo snapshotVipLevel, Guid createdUserId, string createdUsername, CancellationToken cancellationToken = default);
	ValueTask PendingAsync(Guid id, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default);
	ValueTask ApprovedAsync(Guid id, decimal amount, string remark, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default);
	ValueTask RejectAsync(Guid id, string remark, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default);
	ValueTask<bool> ExistAsync(string playerCode, int vipLevel, CancellationToken cancellationToken = default);
	ValueTask<PlayerVipLevelUpgradeFormInfo?> GetAsync(Guid id, CancellationToken cancellationToken = default);
}
