using Tgs.ServiceUtility;

namespace Tgs.VipLevelService.ApplicationServices;

internal class VipLevelQueryService(
	IVipLevelRepository repository) : IQueryService<VipLevelQuery, VipLevelInfo>
{
	public IAsyncEnumerable<VipLevelInfo> QueryAsync(VipLevelQuery query, CancellationToken cancellationToken = default)
		=> repository.QueryAsync(
			brandId: query.BrandId,
			cancellationToken: cancellationToken);
}
