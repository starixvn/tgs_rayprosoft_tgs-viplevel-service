using System.Text.Json;
using StackExchange.Redis;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Repositories;
internal class PlayerVipLevelUpgradePromotionCacheRepository(
	IPlayerVipLevelUpgradePromotionRepository repository,
	IDatabase database,
	TimeSpan expiry) : IPlayerVipLevelUpgradePromotionRepository
{
	public async ValueTask AddAsync(Guid id, Guid snapshotId, Guid vipLevelId, string playerCode, decimal amount, CancellationToken cancellationToken = default)
	{
		await repository.AddAsync(
			id: id,
			snapshotId: snapshotId,
			vipLevelId: vipLevelId, playerCode: playerCode,
			amount: amount,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await SetAsync(
			playerCode: playerCode,
			vipLevelId: vipLevelId)
			.ConfigureAwait(false);
	}

	public async ValueTask<bool> ExistAsync(Guid vipLevelId, string playerCode, CancellationToken cancellationToken = default)
	=> await database.KeyExistsAsync(
		key: GetKey(playerCode, vipLevelId))
		.ConfigureAwait(false)
		|| await repository.ExistAsync(vipLevelId, playerCode, cancellationToken)
		.ConfigureAwait(false);

	private static string GetKey(string playerCode, Guid vipLevelId)
		=> string.Concat(typeof(PlayerVipLevelUpgradePromotionCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", typeof(PlayerVipLevelUpgradePromotionCacheRepository).Name!.ToLower(), ":", playerCode, ":", vipLevelId);

	private async ValueTask<bool> SetAsync(string playerCode, Guid vipLevelId)
		=> await database.StringSetAsync(
			key: GetKey(playerCode, vipLevelId),
			value: JsonSerializer.Serialize(new
			{
				PlayerCode = playerCode,
				VipId = vipLevelId,
				LogOn = DateTimeOffset.UtcNow
			}),
			expiry: expiry)
		.ConfigureAwait(false);
}
