using System.Text.Json;
using StackExchange.Redis;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Repositories;

internal class PlayerCacheRepository(
	IDatabase database,
	IPlayerRepository repository,
	TimeSpan Expiry) : IPlayerRepository
{
	public async ValueTask<PlayerInfo> GetAsync(string playerCode, CancellationToken cancellationToken = default)
	{
		var info = await GetAsync(playerCode)
			.ConfigureAwait(false);

		if (info is null)
		{
			info = await repository.GetAsync(
				playerCode: playerCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			if (info is not null)
			{
				_ = await SetAsync(
					playerCode: playerCode,
					source: info)
					.ConfigureAwait(false);
			}
		}

		return info!;
	}

	public ValueTask UpdateAsync(string playerCode, Guid vipLevelId, CancellationToken cancellationToken = default)
		=> repository.UpdateAsync(playerCode, vipLevelId, cancellationToken);

	public ValueTask<PlayerVipIdInfo> GetVipIdAsync(string playerCode, CancellationToken cancellationToken = default)
		=> repository.GetVipIdAsync(playerCode, cancellationToken);

	private static string GetKey(string playerCode)
			=> string.Concat(typeof(PlayerCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", typeof(PlayerCacheRepository).Name!.ToLower(), ":", playerCode);

	private async ValueTask<PlayerInfo?> GetAsync(string playerCode)
	{
		var key = GetKey(playerCode);
		if (await database.KeyExistsAsync(key).ConfigureAwait(false))
		{
			var value = await database.StringGetAsync(
				key: key)
				.ConfigureAwait(false);

			if (!value.IsNullOrEmpty)
			{
				return JsonSerializer.Deserialize<PlayerInfo>(value.ToString());
			}
		}

		return null;
	}

	private async ValueTask<bool> SetAsync(string playerCode, PlayerInfo source)
		=> await database.StringSetAsync(
			key: GetKey(playerCode),
			value: JsonSerializer.Serialize(source),
			expiry: Expiry)
		.ConfigureAwait(false);
}
