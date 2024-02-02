using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService;

public interface IPlayerVipLevelExpRepository
{
	ValueTask<PlayerVipLevelExpInfo?> GetAsync(string playerCode, CancellationToken cancellationToken = default);
}
