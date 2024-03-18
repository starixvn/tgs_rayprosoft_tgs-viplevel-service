using Tgs.ServiceUtility;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;

internal class VipLevelSingleByLevelGetService(IVipLevelRepository repository) : IGetService<VipLevelSingleByLevelQuery, VipLevelInfo?>
{
	public ValueTask<VipLevelInfo?> GetAsync(VipLevelSingleByLevelQuery query, CancellationToken cancellationToken = default)
		=> repository.GetAsync(
			brandId: query.BrandId,
			level: query.Level,
			cancellationToken: cancellationToken);
}
