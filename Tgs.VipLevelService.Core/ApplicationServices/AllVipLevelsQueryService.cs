using Tgs.ServiceUtility;

namespace Tgs.VipLevelService.ApplicationServices;

internal class AllVipLevelsQueryService : IQueryService<AllVipLevelsQuery, VipLevelInfo>
{
	private readonly ITgsRepository _tgsRepository;

	public AllVipLevelsQueryService(ITgsRepository tgsRepository)
	{
		_tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
	}

	public IAsyncEnumerable<VipLevelInfo> QueryAsync(AllVipLevelsQuery query, CancellationToken cancellationToken = default)
		=> _tgsRepository.QueryAllVipLevelsAsync(query.BrandId, cancellationToken);
}
