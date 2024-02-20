using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService;

public interface IPlayerVipLevelExpFormRepository
{
	ValueTask CreateAsync(Guid id, Guid brandId, string playerCode, int vipLevel, PlayerVipLevelExpFormType type, bool autoState, int sign, decimal amount, PlayerVipLevelExpFormSnapshotExpInfo snapshotExp, string remark, Guid createdUserId, string createdUsername, CancellationToken cancellationToken = default);

	ValueTask PendingAsync(Guid id, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default);

	ValueTask ApprovedAsync(Guid id, decimal amount, string remark, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default);

	ValueTask RejectAsync(Guid id, string remark, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default);

	ValueTask<PlayerVipLevelExpFormInfo?> GetAsync(Guid id, CancellationToken cancellationToken = default);
}
