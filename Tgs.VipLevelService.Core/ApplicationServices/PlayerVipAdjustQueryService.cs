using Tgs.ServiceUtility;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PlayerVipAdjustQueryService
	: IQueryService<PlayerVipAdjustQuery, PlayerVipAdjustInfo>, IGetService<PlayerVipAdjustQuery, int>
{
	private readonly ITgsRepository _tgsRepository;

	public PlayerVipAdjustQueryService(ITgsRepository tgsRepository)
	{
		_tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
	}

	public ValueTask<int> GetAsync(PlayerVipAdjustQuery query, CancellationToken cancellationToken = default)
		=> _tgsRepository.CountPlayerVipAdjustInfosAsync(
			  new PlayerVipAdjustQueryConditions(
				query.BrandId,
				query.PlayerCode),
			  cancellationToken);

	public IAsyncEnumerable<PlayerVipAdjustInfo> QueryAsync(PlayerVipAdjustQuery query, CancellationToken cancellationToken = default)
		=> _tgsRepository.QueryPlayerVipAdjustInfosAsync(
			  new PlayerVipAdjustQueryConditions(
				query.BrandId,
				query.PlayerCode),
			query.Paging,
			cancellationToken);
}
