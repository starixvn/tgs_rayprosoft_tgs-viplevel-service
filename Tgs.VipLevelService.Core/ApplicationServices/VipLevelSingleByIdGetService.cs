using Tgs.ServiceUtility;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;

internal class VipLevelSingleByIdGetService(IVipLevelRepository repository) : IGetService<VipLevelSingleByIdQuery, VipLevelInfo?>
{
	public ValueTask<VipLevelInfo?> GetAsync(VipLevelSingleByIdQuery query, CancellationToken cancellationToken = default)
		=> repository.GetAsync(
			id: query.Id,
			cancellationToken: cancellationToken);
}
