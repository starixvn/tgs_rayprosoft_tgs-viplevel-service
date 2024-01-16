using Tgs.ServiceUtility;

namespace Tgs.VipLevelService.ApplicationServices;

internal class GetVipLevelBaseModifyModelQueryService(ITgsRepository tgsRepository) : IGetService<GetVipLevelBaseModifyModelQuery, VipLevelUpdate?>
{
	public ValueTask<VipLevelUpdate?> GetAsync(GetVipLevelBaseModifyModelQuery query, CancellationToken cancellationToken = default)
		=> tgsRepository.GetVipLevelBaseModifyModelAsync(query.Id, cancellationToken);
}
