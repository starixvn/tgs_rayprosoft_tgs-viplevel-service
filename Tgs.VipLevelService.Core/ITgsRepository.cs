namespace Tgs.VipLevelService;

public interface ITgsRepository
{
	IAsyncEnumerable<VipLevelInfo> QueryAllVipLevelsAsync(Guid brandId, CancellationToken cancellationToken = default);

	ValueTask<VipLevelModifyModel?> GetVipLevelBaseModifyModelAsync(Guid id, CancellationToken cancellationToken = default);

	ValueTask ModifyVipLevelAsync(Guid id, VipLevelModifyModel modifyModel, string modifier, CancellationToken cancellationToken = default);

	ValueTask CreateVipLevelAsync(Guid id, Guid brandId, VipLevelCreateModel createModel, string modifier, CancellationToken cancellationToken = default);
}
