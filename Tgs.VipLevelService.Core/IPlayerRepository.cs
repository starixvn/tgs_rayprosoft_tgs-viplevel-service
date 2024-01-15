using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService;

public interface IPlayerRepository
{
	ValueTask UpdateAsync(string playerCode, Guid vipLevelId, CancellationToken cancellationToken = default);

	ValueTask<PlayerInfo> GetAsync(string playerCode, CancellationToken cancellationToken = default);

	ValueTask<PlayerVipIdInfo> GetVipIdAsync(string playerCode, CancellationToken cancellationToken = default);
}
