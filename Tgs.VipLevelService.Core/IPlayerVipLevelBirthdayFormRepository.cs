using System.Text.Json.Nodes;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService;

public interface IPlayerVipLevelBirthdayFormRepository
{
	ValueTask CreateAsync(Guid id, Guid brandId, string playerCode, int vipLevel, DateOnly birthday, bool autoState, decimal amount, PlayerVipLevelBirthdaySnapshotVipLevelInfo snapshotVipLevel, Guid createdUserId, string createdUsername, CancellationToken cancellationToken = default);
	ValueTask PendingAsync(Guid id, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default);
	ValueTask ApprovedAsync(Guid id, decimal amount, string remark, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default);
	ValueTask RejectAsync(Guid id, string remark, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default);
	ValueTask<bool> ExistAsync(string playerCode, DateOnly birthday, CancellationToken cancellationToken = default);
	ValueTask<PlayerVipLevelBirthdayFormInfo?> GetAsync(Guid id, CancellationToken cancellationToken = default);
}
