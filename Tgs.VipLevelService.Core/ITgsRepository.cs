using Tgs.Paging;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService;

public interface ITgsRepository
{
	IAsyncEnumerable<VipLevelInfo> QueryAllVipLevelsAsync(Guid brandId, CancellationToken cancellationToken = default);

	ValueTask<VipLevelUpdate?> GetVipLevelBaseModifyModelAsync(Guid id, CancellationToken cancellationToken = default);

	ValueTask ModifyVipLevelAsync(Guid id, VipLevelUpdate modifyModel, string modifier, CancellationToken cancellationToken = default);

	ValueTask CreateVipLevelAsync(Guid id, Guid brandId, VipLevelCreate createModel, string modifier, CancellationToken cancellationToken = default);

	IAsyncEnumerable<PlayerVipAdjustInfo> QueryPlayerVipAdjustInfosAsync(
		PlayerVipAdjustQueryConditions conditions,
		Paging<PlayerVipAdjustInfo> paging,
		CancellationToken cancellationToken = default);

	ValueTask<int> CountPlayerVipAdjustInfosAsync(
		PlayerVipAdjustQueryConditions conditions,
		CancellationToken cancellationToken = default);
}
