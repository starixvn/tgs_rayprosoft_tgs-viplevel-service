using Microsoft.EntityFrameworkCore;
using Tgs.VipLevelService.Entities;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Repositories;

internal class PlayerVipLevelExpRepository(
	TgsContext context) : IPlayerVipLevelExpRepository
{
	public ValueTask<PlayerVipLevelExpInfo?> GetAsync(string playerCode, CancellationToken cancellationToken = default)
		=> context.PlayerVipLevelExps
		.Where(predicate: predicate => predicate.PlayerCode == playerCode)
		.Select(selector: selector => new PlayerVipLevelExpInfo(
			selector.PlayerCode,
			selector.AccumDepositId,
			selector.AccumDeposit,
			selector.AccumTurnoverId,
			selector.AccumTurnover))
		.ToAsyncEnumerable()
		.FirstOrDefaultAsync(
			cancellationToken: cancellationToken);
}
