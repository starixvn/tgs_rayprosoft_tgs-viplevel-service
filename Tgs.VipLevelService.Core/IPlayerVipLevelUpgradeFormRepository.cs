using System.Text.Json.Nodes;
using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService;

public interface IPlayerVipLevelUpgradeFormRepository
{
	ValueTask CreateAsync(Guid id, Guid brandId, string playerCode, int vipLevel, bool autoState, decimal amount, PlayerVipLevelUpgradeFormSnapshotVipLevelInfo snapshotVipLevel, Guid createdUserId, string createdUsername, CancellationToken cancellationToken = default);

	ValueTask PendingAsync(Guid id, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default);

	ValueTask ApprovedAsync(Guid id, decimal amount, string remark, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default);

	ValueTask RejectAsync(Guid id, string remark, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default);

	ValueTask<bool> ExistAsync(string playerCode, int vipLevel, CancellationToken cancellationToken = default);

	ValueTask<PlayerVipLevelUpgradeFormInfo?> GetAsync(Guid id, CancellationToken cancellationToken = default);

	IAsyncEnumerable<PlayerVipLevelUpgradeFormQueryInfo> QueryAsync(Guid brandId, DateTimeOffset updatedOnStart, DateTimeOffset updatedOnEnd, string playerCode, string username, IEnumerable<PlayerVipLevelUpgradeFormState> states, int offset = 0, int limit = 20, CancellationToken cancellationToken = default);

	ValueTask<int> QueryCountAsync(Guid brandId, DateTimeOffset updatedOnStart, DateTimeOffset updatedOnEnd, string playerCode, string username, IEnumerable<PlayerVipLevelUpgradeFormState> states, CancellationToken cancellationToken = default);
}
