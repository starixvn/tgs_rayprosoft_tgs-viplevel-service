using Tgs.ServiceUtility;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;
internal class PlayerVipLevelExpFixesGetService(
	IPlayerVipLevelExpRepository repository) : IGetService<PlayerVipLevelExpFixesQuery, PlayerVipLevelExpInfo>
{
	public async ValueTask<PlayerVipLevelExpInfo> GetAsync(PlayerVipLevelExpFixesQuery query, CancellationToken cancellationToken = default)
	{
		await repository.CreateAsync(
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return await repository.GetAsync(
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new NullReferenceException();
	}
}
