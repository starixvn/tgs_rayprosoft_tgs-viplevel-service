using System.Runtime.CompilerServices;
using System.Text.Json;
using StackExchange.Redis;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Repositories;

internal class VipLevelCacheRepository(
	IDatabase database,
	IVipLevelRepository repository,
	TimeSpan Expiry) : IVipLevelRepository
{
	public async IAsyncEnumerable<VipLevelInfo> QueryAsync(Guid brandId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var vipLevelInfos = await CacheQueryAsync(brandId)
			.ToArrayAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? Enumerable.Empty<VipLevelInfo>();

		if (!vipLevelInfos.Any())
		{
			vipLevelInfos = await repository.QueryAsync(
				brandId: brandId,
				cancellationToken: cancellationToken)
				.ToArrayAsync(
				cancellationToken: cancellationToken)
				.ConfigureAwait(false)
				?? Enumerable.Empty<VipLevelInfo>();
			if (vipLevelInfos.Any())
			{
				_ = await SetQueryAsync(
					brandId: brandId,
					source: vipLevelInfos)
					.ConfigureAwait(false);
			}
		}

		if (!vipLevelInfos.Any())
		{
			yield break;
		}

		foreach (var vipLevelInfo in vipLevelInfos)
		{
			yield return vipLevelInfo;
		}
	}

	public async ValueTask<VipLevelInfo?> GetAsync(Guid brandId, Guid id, CancellationToken cancellationToken = default)
	{
		var vipLevelInfos = await QueryAsync(
			brandId: brandId,
			cancellationToken: cancellationToken)
			.ToArrayAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? Enumerable.Empty<VipLevelInfo>();

		return vipLevelInfos.FirstOrDefault(predicate: vl => vl.BrandId == brandId && vl.Id == id);
	}

	public async ValueTask<VipLevelInfo?> GetAsync(Guid brandId, int level, CancellationToken cancellationToken = default)
	{
		var vipLevelInfos = await QueryAsync(
			brandId: brandId,
			cancellationToken: cancellationToken)
			.ToArrayAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? Enumerable.Empty<VipLevelInfo>();

		return vipLevelInfos.FirstOrDefault(predicate: vl => vl.BrandId == brandId && vl.Level == level);
	}

	public async ValueTask<VipLevelInfo?> GetAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var vipLevelInfo = await CacheVipIdAsync(id)
			.ConfigureAwait(false);

		if (vipLevelInfo is null)
		{
			vipLevelInfo = await repository.GetAsync(
				id: id,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			if (vipLevelInfo is not null)
			{
				await SetVipIdAsync(
					vipId: id,
					vipLevelInfo)
					.ConfigureAwait(false);
			}
		}

		return vipLevelInfo;
	}

	public async ValueTask UpdateAsync(Guid id, Guid brandId, VipLevelUpdate update, string modifier, CancellationToken cancellationToken = default)
	{
		await repository.UpdateAsync(
			id: id,
			brandId: brandId,
			update: update,
			modifier: modifier,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await DeleteQueryAsync(
			brandId: brandId)
			.ConfigureAwait(false);
		await DeleteVipIdAsync(
			vipId: id)
			.ConfigureAwait(false);
	}

	public async ValueTask CreateAsync(Guid id, Guid brandId, VipLevelCreate create, string modifier, CancellationToken cancellationToken = default)
	{
		await repository.CreateAsync(
			id: id,
			brandId: brandId,
			create: create,
			modifier: modifier,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await DeleteQueryAsync(
			brandId: brandId)
			.ConfigureAwait(false);
		await DeleteVipIdAsync(
			vipId: id)
			.ConfigureAwait(false);
	}

	private static string GetQueryKey(Guid brandId)
		=> string.Concat(typeof(VipLevelCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", typeof(VipLevelCacheRepository).Name!.ToLower(), ":query:", brandId);

	private static string GetVipIdKey(Guid vipId)
		=> string.Concat(typeof(VipLevelCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", typeof(VipLevelCacheRepository).Name!.ToLower(), ":get:", vipId);

	private async IAsyncEnumerable<VipLevelInfo> CacheQueryAsync(Guid brandId)
	{
		var key = GetQueryKey(brandId);
		var vipLevelInfos = Enumerable.Empty<VipLevelInfo>();
		if (await database.KeyExistsAsync(key).ConfigureAwait(false))
		{
			var value = await database.StringGetAsync(
				key: key)
				.ConfigureAwait(false);

			if (!value.IsNullOrEmpty)
			{
				vipLevelInfos = JsonSerializer.Deserialize<IEnumerable<VipLevelInfo>>(value.ToString())
					?? Enumerable.Empty<VipLevelInfo>();
			}
		}

		if (!vipLevelInfos.Any())
		{
			yield break;
		}

		foreach (var vipLevelInfo in vipLevelInfos)
		{
			yield return vipLevelInfo;
		}
	}

	private async ValueTask<VipLevelInfo?> CacheVipIdAsync(Guid vipId)
	{
		var key = GetVipIdKey(vipId);
		var vipLevelInfo = default(VipLevelInfo);
		if (await database.KeyExistsAsync(key).ConfigureAwait(false))
		{
			var value = await database.StringGetAsync(
				key: key)
				.ConfigureAwait(false);

			if (!value.IsNullOrEmpty)
			{
				vipLevelInfo = JsonSerializer.Deserialize<VipLevelInfo>(value.ToString());
			}
		}

		return vipLevelInfo;
	}

	private async ValueTask<bool> SetQueryAsync(Guid brandId, IEnumerable<VipLevelInfo> source)
		=> await database.StringSetAsync(
			key: GetQueryKey(brandId),
			value: JsonSerializer.Serialize(source),
			expiry: Expiry)
		.ConfigureAwait(false);

	private async ValueTask<bool> DeleteQueryAsync(Guid brandId)
		=> await database.KeyDeleteAsync(
			key: GetQueryKey(brandId),
			flags: CommandFlags.FireAndForget)
		.ConfigureAwait(false);

	private async ValueTask<bool> SetVipIdAsync(Guid vipId, VipLevelInfo source)
		=> await database.StringSetAsync(
			key: GetVipIdKey(vipId),
			value: JsonSerializer.Serialize(source),
			expiry: Expiry)
		.ConfigureAwait(false);

	private async ValueTask<bool> DeleteVipIdAsync(Guid vipId)
		=> await database.KeyDeleteAsync(
			key: GetVipIdKey(vipId),
			flags: CommandFlags.FireAndForget)
		.ConfigureAwait(false);
}
