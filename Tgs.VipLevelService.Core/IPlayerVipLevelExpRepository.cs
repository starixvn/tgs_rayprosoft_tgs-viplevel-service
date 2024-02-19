using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService;

public interface IPlayerVipLevelExpRepository
{
	ValueTask CreateAsync(string playerCode, CancellationToken cancellationToken = default);

	ValueTask<PlayerVipLevelExpInfo?> GetAsync(string playerCode, CancellationToken cancellationToken = default);

	ValueTask UpdateDepositAsync(string playerCode, decimal accumDeposit, CancellationToken cancellationToken = default);

	ValueTask UpdateTurnoverAsync(string playerCode, decimal accumTurnover, CancellationToken cancellationToken = default);
}
