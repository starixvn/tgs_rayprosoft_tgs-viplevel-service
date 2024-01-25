using Tgs.ServiceUtility;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PlayerVipLevelExpGetService(
	IPlayerVipLevelExpRepository repository) : IGetService<PlayerVipLevelExpGetQuery, PlayerVipLevelExpInfo?>
{
	public ValueTask<PlayerVipLevelExpInfo?> GetAsync(PlayerVipLevelExpGetQuery query, CancellationToken cancellationToken = default)
		=> repository.GetAsync(query.PlayerCode, cancellationToken);
}
