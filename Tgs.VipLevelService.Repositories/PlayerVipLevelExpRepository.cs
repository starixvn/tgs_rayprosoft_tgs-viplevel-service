using Microsoft.EntityFrameworkCore;
using Tgs.VipLevelService.Entities;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Repositories;

internal class PlayerVipLevelExpRepository(
	TgsContext context) : IPlayerVipLevelExpRepository
{
	public async ValueTask CreateAsync(string playerCode, CancellationToken cancellationToken = default)
	{
		var exp = await context.PlayerVipLevelExps
			.FirstOrDefaultAsync(
			predicate: exp => exp.PlayerCode == playerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (exp is not null)
		{
			return;
		}

		exp = new PlayerVipLevelExp
		{
			PlayerCode = playerCode,
			AccumDepositId = Guid.NewGuid(),
			AccumDeposit = 0.0m,
			AccumTurnoverId = Guid.NewGuid(),
			AccumTurnover = 0.0m
		};

		await context.PlayerVipLevelExps
			.AddAsync(
			entity: exp,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await context.SaveChangesAsync(
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
	public async ValueTask UpdateDepositAsync(string playerCode, decimal accumDeposit, CancellationToken cancellationToken = default)
	{
		var exp = await context.PlayerVipLevelExps.SingleAsync(
			predicate: exp => exp.PlayerCode == playerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		exp.AccumDeposit = accumDeposit;

		await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
	public async ValueTask UpdateTurnoverAsync(string playerCode, decimal accumTurnover, CancellationToken cancellationToken = default)
	{
		var exp = await context.PlayerVipLevelExps.SingleAsync(
			predicate: exp => exp.PlayerCode == playerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		exp.AccumTurnover = accumTurnover;

		await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
