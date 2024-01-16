using Tgs.ServiceUtility;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;

internal class AllVipLevelsQueryService(ITgsRepository tgsRepository) : IQueryService<AllVipLevelsQuery, VipLevelInfo>
{
	public IAsyncEnumerable<VipLevelInfo> QueryAsync(AllVipLevelsQuery query, CancellationToken cancellationToken = default)
		=> tgsRepository.QueryAllVipLevelsAsync(query.BrandId, cancellationToken);
}
