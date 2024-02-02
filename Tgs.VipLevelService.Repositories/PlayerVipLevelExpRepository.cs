using Microsoft.EntityFrameworkCore;
using Tgs.VipLevelService.Entities;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Repositories;

internal class PlayerVipLevelExpRepository(
	TgsContext context) : IPlayerVipLevelExpRepository
{
	public async ValueTask CreateAsync(string playerCode, CancellationToken cancellationToken = default)
	{
		var playerVipLevelExp = await context.PlayerVipLevelExps
			.FirstOrDefaultAsync(
			predicate: vle => vle.PlayerCode == playerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (playerVipLevelExp is null)
		{
			return;
		}

		playerVipLevelExp = new PlayerVipLevelExp
		{
			PlayerCode = playerCode,
			AccumDepositId = Guid.NewGuid(),
			AccumDeposit = 0.0m,
			AccumTurnoverId = Guid.NewGuid(),
			AccumTurnover = 0.0m
		};

		_ = await context.PlayerVipLevelExps
			.AddAsync(
			entity: playerVipLevelExp,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
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
