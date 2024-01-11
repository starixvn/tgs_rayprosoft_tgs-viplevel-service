using Tgs.ServiceUtility;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PlayerVipAdjustQueryService(ITgsRepository tgsRepository)
		: IQueryService<PlayerVipAdjustQuery, PlayerVipAdjustInfo>, IGetService<PlayerVipAdjustQuery, int>
{
	public ValueTask<int> GetAsync(PlayerVipAdjustQuery query, CancellationToken cancellationToken = default)
		=> tgsRepository.CountPlayerVipAdjustInfosAsync(
			  new PlayerVipAdjustQueryConditions(
				query.PlayerCode,
				query.CreateOnRange),
			  cancellationToken);

	public IAsyncEnumerable<PlayerVipAdjustInfo> QueryAsync(PlayerVipAdjustQuery query, CancellationToken cancellationToken = default)
		=> tgsRepository.QueryPlayerVipAdjustInfosAsync(
			  new PlayerVipAdjustQueryConditions(
				query.PlayerCode,
				query.CreateOnRange),
			query.Paging,
			cancellationToken);
}
