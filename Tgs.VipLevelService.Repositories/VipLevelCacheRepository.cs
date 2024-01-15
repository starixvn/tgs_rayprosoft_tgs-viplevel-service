using System.Runtime.CompilerServices;
using System.Text.Json;
using StackExchange.Redis;

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
				_ = await SetAsync(
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

	public ValueTask<VipLevelInfo?> GetAsync(Guid id, CancellationToken cancellationToken = default)
		=> repository.GetAsync(
			id: id,
			cancellationToken: cancellationToken);

	public async ValueTask UpdateAsync(Guid id, Guid brandId, VipLevelModifyModel modifyModel, string modifier, CancellationToken cancellationToken = default)
	{
		await repository.UpdateAsync(
			id: id,
			brandId: brandId,
			modifyModel: modifyModel,
			modifier: modifier,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await DeleteAsync(
			brandId: brandId)
			.ConfigureAwait(false);
	}

	public async ValueTask CreateAsync(Guid id, Guid brandId, VipLevelCreateModel createModel, string modifier, CancellationToken cancellationToken = default)
	{
		await repository.CreateAsync(
			id: id,
			brandId: brandId,
			createModel: createModel,
			modifier: modifier,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await DeleteAsync(
			brandId: brandId)
			.ConfigureAwait(false);
	}

	private static string GetKey(Guid brandId)
								=> string.Concat(typeof(VipLevelCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", typeof(VipLevelCacheRepository).Name!.ToLower(), ":", brandId);

	private async IAsyncEnumerable<VipLevelInfo> CacheQueryAsync(Guid brandId)
	{
		var key = GetKey(brandId);
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

	private async ValueTask<bool> SetAsync(Guid brandId, IEnumerable<VipLevelInfo> source)
		=> await database.StringSetAsync(
			key: GetKey(brandId),
			value: JsonSerializer.Serialize(source),
			expiry: Expiry)
		.ConfigureAwait(false);

	private async ValueTask<bool> DeleteAsync(Guid brandId)
		=> await database.KeyDeleteAsync(
			key: GetKey(brandId),
			flags: CommandFlags.FireAndForget)
		.ConfigureAwait(false);
}
