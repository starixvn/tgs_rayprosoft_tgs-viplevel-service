using System.Text.Json;
using StackExchange.Redis;
using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Repositories;

internal class PlayerVipLevelCacheRepository(
	IPlayerVipLevelRepository repository,
	IDatabase database,
	TimeSpan Expiry) : IPlayerVipLevelRepository
{
	public async ValueTask<PlayerVipLevelInfo?> GetAsync(string playerCode, CancellationToken cancellationToken = default)
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

		return info;
	}

	public async ValueTask MergeAsync(string playerCode, Guid vipLevelId, int vipLevel, PlayerVipLevelAutoState autoState, CancellationToken cancellationToken = default)
	{
		await repository.MergeAsync(
			playerCode: playerCode,
			vipLevelId: vipLevelId,
			vipLevel: vipLevel,
			autoState: autoState,
			cancellationToken: cancellationToken)
			 .ConfigureAwait(false);

		_ = await DeleteAsync(
			playerCode: playerCode)
			.ConfigureAwait(false);
	}

	public async ValueTask UpdateSystemAsync(string playerCode, Guid vipLevelId, int vipLevel, CancellationToken cancellationToken = default)
	{
		await repository.UpdateSystemAsync(
			playerCode: playerCode,
			vipLevelId: vipLevelId,
			vipLevel: vipLevel,
			cancellationToken: cancellationToken)
			 .ConfigureAwait(false);

		_ = await DeleteAsync(
			playerCode: playerCode)
			.ConfigureAwait(false);
	}

	private static string GetKey(string playerCode)
		=> string.Concat(typeof(PlayerVipLevelCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", typeof(PlayerVipLevelCacheRepository).Name!.ToLower(), ":", playerCode);

	private async ValueTask<PlayerVipLevelInfo?> GetAsync(string playerCode)
	{
		var key = GetKey(playerCode);
		if (await database.KeyExistsAsync(key).ConfigureAwait(false))
		{
			var value = await database.StringGetAsync(
				key: key)
				.ConfigureAwait(false);

			if (!value.IsNullOrEmpty)
			{
				return JsonSerializer.Deserialize<PlayerVipLevelInfo>(value.ToString());
			}
		}

		return null;
	}

	private async ValueTask<bool> SetAsync(string playerCode, PlayerVipLevelInfo source)
		=> await database.StringSetAsync(
			key: GetKey(playerCode),
			value: JsonSerializer.Serialize(source),
			expiry: Expiry)
		.ConfigureAwait(false);

	private async ValueTask<bool> DeleteAsync(string playerCode)
		=> await database.KeyDeleteAsync(
			key: GetKey(playerCode),
			flags: CommandFlags.FireAndForget)
		.ConfigureAwait(false);
}
