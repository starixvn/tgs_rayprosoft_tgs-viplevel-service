using Tgs.ServiceUtility;

namespace Tgs.VipLevelService.ApplicationServices;

internal class VipLevelByIdGetService(IVipLevelRepository repository) : IGetService<VipLevelByIdQuery, VipLevelInfo?>
{
	public ValueTask<VipLevelInfo?> GetAsync(VipLevelByIdQuery query, CancellationToken cancellationToken = default)
		=> repository.GetAsync(
			brandId: query.BrandId,
			id: query.Id,
			cancellationToken: cancellationToken);
}
