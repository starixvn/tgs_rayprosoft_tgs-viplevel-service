namespace Tgs.VipLevelService.Clients;

public interface IVipLevelServiceClient
{
	ValueTask<Guid> CreateVipLevelAsync(
		Guid brandId,
		VipLevelCreateInfo createInfo,
		string modifier,
		CancellationToken cancellationToken = default);

	IAsyncEnumerable<VipLevelInfo> GetBrandAllVipLevelsAsync(Guid brandId, CancellationToken cancellationToken = default);

	ValueTask UpdateVipLevelAsync(
		Guid vipId,
		VipLevelUpdateInfo updateInfo,
		string modifier,
		CancellationToken cancellationToken = default);

	ValueTask<PagingResponse<PlayerVipAdjustInfo>> QueryPlayerVipAdjustInfosAsync(
		PlayerVipAdjustQueryConditions conditions,
		Paging paging,
		CancellationToken cancellationToken = default);
}
