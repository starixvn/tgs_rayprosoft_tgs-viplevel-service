using System.Text.Json;
using StackExchange.Redis;
using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Repositories;

internal class PlayerVipLevelUpgradeFormCacheRepository(
	IPlayerVipLevelUpgradeFormRepository repository,
	IDatabase database,
	TimeSpan expiry) : IPlayerVipLevelUpgradeFormRepository
{
	public ValueTask ApprovedAsync(Guid id, decimal amount, string remark, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default)
		=> repository.ApprovedAsync(id, amount, remark, updatedUserId, updatedUsername, cancellationToken);

	public async ValueTask CreateAsync(Guid id, Guid brandId, string playerCode, int vipLevel, bool autoState, decimal amount, PlayerVipLevelUpgradeFormSnapshotVipLevelInfo snapshotVipLevel, Guid createdUserId, string createdUsername, CancellationToken cancellationToken = default)
	{
		await SetAsync(playerCode, vipLevel).ConfigureAwait(false);

		await repository.CreateAsync(
			id, brandId, playerCode, vipLevel, autoState, amount, snapshotVipLevel, createdUserId, createdUsername, cancellationToken)
			.ConfigureAwait(false);
		
	}

	public async ValueTask<bool> ExistAsync(string playerCode, int vipLevel, CancellationToken cancellationToken = default)
	=> await database.KeyExistsAsync(
		key: GetKey(playerCode, vipLevel))
		.ConfigureAwait(false)
		|| await repository.ExistAsync(playerCode, vipLevel, cancellationToken)
		.ConfigureAwait(false);

	public ValueTask<PlayerVipLevelUpgradeFormInfo?> GetAsync(Guid id, CancellationToken cancellationToken = default)
		=> repository.GetAsync(id, cancellationToken);

	public ValueTask PendingAsync(Guid id, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default)
		=> repository.PendingAsync(id, updatedUserId, updatedUsername, cancellationToken);

	public ValueTask RejectAsync(Guid id, string remark, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default)
		=> repository.RejectAsync(id, remark, updatedUserId, updatedUsername, cancellationToken);

	public IAsyncEnumerable<PlayerVipLevelUpgradeFormQueryInfo> QueryAsync(Guid brandId, DateTimeOffset updatedOnStart, DateTimeOffset updatedOnEnd, string playerCode, string username, IEnumerable<PlayerVipLevelUpgradeFormState> states, int offset = 0, int limit = 20, CancellationToken cancellationToken = default)
		=> repository.QueryAsync(brandId: brandId, updatedOnStart: updatedOnStart, updatedOnEnd: updatedOnEnd, playerCode: playerCode, username: username, states: states, offset: offset, limit: limit, cancellationToken: cancellationToken);

	public ValueTask<int> QueryCountAsync(Guid brandId, DateTimeOffset updatedOnStart, DateTimeOffset updatedOnEnd, string playerCode, string username, IEnumerable<PlayerVipLevelUpgradeFormState> states, CancellationToken cancellationToken = default)
		=> repository.QueryCountAsync(brandId: brandId, updatedOnStart: updatedOnStart, updatedOnEnd: updatedOnEnd, playerCode: playerCode, username: username, states: states, cancellationToken: cancellationToken);

	private static string GetKey(string playerCode, int vipLevel)
				=> string.Concat(typeof(PlayerVipLevelUpgradeFormCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", typeof(PlayerVipLevelUpgradeFormCacheRepository).Name!.ToLower(), ":", playerCode, ":", vipLevel);

	private async ValueTask<bool> SetAsync(string playerCode, int vipLevel)
		=> await database.StringSetAsync(
			key: GetKey(playerCode, vipLevel),
			value: JsonSerializer.Serialize(new
			{
				PlayerCode = playerCode,
				VipLevel = vipLevel,
				LogOn = DateTimeOffset.UtcNow
			}),
			expiry: expiry,
			when: When.NotExists)
		.ConfigureAwait(false);
}
