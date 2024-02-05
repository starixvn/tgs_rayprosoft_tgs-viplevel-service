using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;
using Tgs.VipLevelService.Entities;

namespace Tgs.VipLevelService.Repositories;
internal class PlayerVipLevelBirthdayPromotionCacheRepository(
	IPlayerVipLevelBirthdayPromotionRepository repository,
	IDatabase database,
	TimeSpan expiry) : IPlayerVipLevelBirthdayPromotionRepository
{
	public async ValueTask AddAsync(Guid id, Guid snapshotId, Guid vipLevelId, Guid brandId, DateOnly birthday, string playerCode, decimal amount, CancellationToken cancellationToken = default)
	{
		await repository.AddAsync(
			id: id,
			snapshotId: snapshotId,
			vipLevelId: vipLevelId,
			brandId: brandId,
			birthday: birthday,
			playerCode: playerCode,
			amount: amount,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await SetAsync(
			playerCode: playerCode,
			birthday: birthday)
			.ConfigureAwait(false);
	}
	public async ValueTask<bool> ExistAsync(DateOnly birthday, string playerCode, CancellationToken cancellationToken = default)
	=> await database.KeyExistsAsync(
		key: GetKey(playerCode, birthday))
		.ConfigureAwait(false)
		|| await repository.ExistAsync(birthday, playerCode, cancellationToken)
		.ConfigureAwait(false);

	private static string GetKey(string playerCode, DateOnly birthday)
		=> string.Concat(typeof(PlayerVipLevelBirthdayPromotionCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", typeof(PlayerVipLevelBirthdayPromotionCacheRepository).Name!.ToLower(), ":", playerCode, ":", birthday.Year);

	private async ValueTask<bool> SetAsync(string playerCode, DateOnly birthday)
		=> await database.StringSetAsync(
			key: GetKey(playerCode, birthday),
			value: JsonSerializer.Serialize(new
			{
				PlayerCode = playerCode,
				Birthday = birthday,
				LogOn = DateTimeOffset.UtcNow
			}),
			expiry: expiry)
		.ConfigureAwait(false);
}
