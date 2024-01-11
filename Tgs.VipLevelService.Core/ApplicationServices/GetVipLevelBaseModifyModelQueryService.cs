using Tgs.ServiceUtility;

namespace Tgs.VipLevelService.ApplicationServices;

internal class GetVipLevelBaseModifyModelQueryService(ITgsRepository tgsRepository) : IGetService<GetVipLevelBaseModifyModelQuery, VipLevelModifyModel?>
{
	public ValueTask<VipLevelModifyModel?> GetAsync(GetVipLevelBaseModifyModelQuery query, CancellationToken cancellationToken = default)
		=> tgsRepository.GetVipLevelBaseModifyModelAsync(query.Id, cancellationToken);
}
