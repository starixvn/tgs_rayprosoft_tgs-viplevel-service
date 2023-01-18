using Tgs.Paging;

namespace Tgs.VipLevelService;

public interface ITgsRepository
{
	IAsyncEnumerable<VipLevelInfo> QueryAllVipLevelsAsync(Guid brandId, CancellationToken cancellationToken = default);

	ValueTask<VipLevelModifyModel?> GetVipLevelBaseModifyModelAsync(Guid id, CancellationToken cancellationToken = default);

	ValueTask ModifyVipLevelAsync(Guid id, VipLevelModifyModel modifyModel, string modifier, CancellationToken cancellationToken = default);

	ValueTask CreateVipLevelAsync(Guid id, Guid brandId, VipLevelCreateModel createModel, string modifier, CancellationToken cancellationToken = default);

	IAsyncEnumerable<PlayerVipAdjustInfo> QueryPlayerVipAdjustInfosAsync(
		PlayerVipAdjustQueryConditions conditions,
		Paging<PlayerVipAdjustInfo> paging,
		CancellationToken cancellationToken = default);

	ValueTask<int> CountPlayerVipAdjustInfosAsync(
		PlayerVipAdjustQueryConditions conditions,
		CancellationToken cancellationToken = default);
}
