using Tgs.ServiceUtility;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;
internal class PlayerVipLevelPlayerCodeGetService(
	IPlayerVipLevelRepository playerVipLevelRepository,
	IGetService<PlayerVipLevelFixesQuery, PlayerVipLevelInfo> fixesService) : IGetService<PlayerVipLevelPlayerCodeQuery, PlayerVipLevelInfo>
{
	public async ValueTask<PlayerVipLevelInfo> GetAsync(PlayerVipLevelPlayerCodeQuery query, CancellationToken cancellationToken = default)
		=> await playerVipLevelRepository.GetAsync(
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
		.ConfigureAwait(false)
		?? await fixesService.GetAsync(
			query: new PlayerVipLevelFixesQuery(
				PlayerCode: query.PlayerCode),
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);
}
