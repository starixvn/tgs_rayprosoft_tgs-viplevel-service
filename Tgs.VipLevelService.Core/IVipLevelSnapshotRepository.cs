using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService;

public interface IVipLevelSnapshotRepository
{
	ValueTask CreateAsync(Guid id, Guid brandId, IEnumerable<VipLevelInfo> data, DateTimeOffset effectiveOn, Guid snapshotUserId = default, string snapshotUsername = "", CancellationToken cancellationToken = default);

	ValueTask<VipLevelSnapshotInfo?> GetAsync(Guid id, CancellationToken cancellationToken = default);

	ValueTask<Guid?> GetCurrentSnapshotIdAsync(Guid brandId, CancellationToken cancellationToken = default);
}
