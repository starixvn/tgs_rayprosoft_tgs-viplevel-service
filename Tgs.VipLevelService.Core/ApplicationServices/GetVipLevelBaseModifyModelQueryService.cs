using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;

namespace Tgs.VipLevelService.ApplicationServices;

internal class GetVipLevelBaseModifyModelQueryService : IGetService<GetVipLevelBaseModifyModelQuery, VipLevelModifyModel?>
{
	private readonly ITgsRepository _tgsRepository;

	public GetVipLevelBaseModifyModelQueryService(ITgsRepository tgsRepository)
	{
		_tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
	}

	public ValueTask<VipLevelModifyModel?> GetAsync(GetVipLevelBaseModifyModelQuery query, CancellationToken cancellationToken = default)
		=> _tgsRepository.GetVipLevelBaseModifyModelAsync(query.Id, cancellationToken);
}
