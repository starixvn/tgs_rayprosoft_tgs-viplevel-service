using Microsoft.EntityFrameworkCore;
using Tgs.VipLevelService.Common;
using Tgs.VipLevelService.Entities;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Repositories;

internal class PlayerVipLevelRepository(
	TgsContext context) : IPlayerVipLevelRepository
{
	public async ValueTask<PlayerVipLevelInfo?> GetAsync(string playerCode, CancellationToken cancellationToken = default)
	=> await context.PlayerVipLevels
		.Where(predicate: predicate => predicate.PlayerCode == playerCode)
		.Select(selector: selector => new PlayerVipLevelInfo(
			selector.PlayerCode,
			selector.VipLevelId,
			selector.VipLevel,
			(PlayerVipLevelAutoState)selector.AutoState))
		.FirstOrDefaultAsync(
		cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask MergeAsync(string playerCode, Guid vipLevelId, int vipLevel, PlayerVipLevelAutoState autoState, CancellationToken cancellationToken = default)
	{
		var playerVipLevel = context.PlayerVipLevels
			 .FirstOrDefault(predicate: predicate => predicate.PlayerCode == playerCode);

		if (playerVipLevel is null)
		{
			var entity = new PlayerVipLevel
			{
				PlayerCode = playerCode,
				VipLevelId = vipLevelId,
				VipLevel = vipLevel,
				AutoState = (int)autoState
			};

			_ = await context.PlayerVipLevels.AddAsync(
				entity: entity,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
		else
		{
			playerVipLevel.VipLevelId = vipLevelId;
			playerVipLevel.VipLevel = vipLevel;
			playerVipLevel.AutoState = (int)autoState;

			_ = context.PlayerVipLevels.Update(
				entity: playerVipLevel);
		}

		_ = await context.SaveChangesAsync(
			 cancellationToken: cancellationToken)
			 .ConfigureAwait(false);
	}
}
