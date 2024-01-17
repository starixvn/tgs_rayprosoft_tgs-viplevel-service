using Tgs.ServiceUtility;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;

internal class VipLevelByIdGetService(IVipLevelRepository repository) : IGetService<VipLevelByIdQuery, VipLevelInfo?>
{
	public ValueTask<VipLevelInfo?> GetAsync(VipLevelByIdQuery query, CancellationToken cancellationToken = default)
		=> repository.GetAsync(
			id: query.Id,
			cancellationToken: cancellationToken);
}
