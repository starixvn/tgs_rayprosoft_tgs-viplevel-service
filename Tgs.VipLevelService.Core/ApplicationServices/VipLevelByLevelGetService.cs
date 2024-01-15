using Tgs.ServiceUtility;

namespace Tgs.VipLevelService.ApplicationServices;

internal class VipLevelByLevelGetService(IVipLevelRepository repository) : IGetService<VipLevelByLevelQuery, VipLevelInfo?>
{
	public ValueTask<VipLevelInfo?> GetAsync(VipLevelByLevelQuery query, CancellationToken cancellationToken = default)
		=> repository.GetAsync(
			brandId: query.BrandId,
			level: query.Level,
			cancellationToken: cancellationToken);
}
