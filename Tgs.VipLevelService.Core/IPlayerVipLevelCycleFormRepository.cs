using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService;

public interface IPlayerVipLevelCycleFormRepository
{
	ValueTask CreateAsync(Guid id, Guid brandId, string playerCode, int vipLevel, PlayerVipLevelCycleFormCycleType cycleType, DateOnly cycleOn, DateOnly fromOn, DateOnly toOn, bool autoState, decimal amount, PlayerVipLevelCycleFormSnapshotVipLevelInfo snapshotVipLevel, Guid createdUserId, string createdUsername, CancellationToken cancellationToken = default);

	ValueTask PendingAsync(Guid id, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default);

	ValueTask ApprovedAsync(Guid id, decimal amount, string remark, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default);

	ValueTask RejectAsync(Guid id, string remark, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default);

	ValueTask<bool> ExistAsync(string playerCode, PlayerVipLevelCycleFormCycleType cycleType, DateOnly cycleOn, CancellationToken cancellationToken = default);

	ValueTask<PlayerVipLevelCycleFormInfo?> GetAsync(Guid id, CancellationToken cancellationToken = default);
}
