using System.Text.Json;
using StackExchange.Redis;
using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Repositories;

internal class PlayerVipLevelCycleFormCacheRepository(
	IPlayerVipLevelCycleFormRepository repository,
	IDatabase database,
	TimeSpan expiry) : IPlayerVipLevelCycleFormRepository
{
	public ValueTask ApprovedAsync(Guid id, decimal amount, string remark, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default)
		=> repository.ApprovedAsync(id, amount, remark, updatedUserId, updatedUsername, cancellationToken);

	public async ValueTask CreateAsync(Guid id, Guid brandId, string playerCode, int vipLevel, PlayerVipLevelCycleFormCycleType cycleType, DateOnly cycleOn, DateOnly fromOn, DateOnly toOn, bool autoState, decimal amount, PlayerVipLevelCycleFormSnapshotVipLevelInfo snapshotVipLevel, Guid createdUserId, string createdUsername, CancellationToken cancellationToken = default)
	{
		await SetAsync(playerCode, cycleType, cycleOn).ConfigureAwait(false);

		await repository.CreateAsync(
			id, brandId, playerCode, vipLevel, cycleType, cycleOn, fromOn, toOn, autoState, amount, snapshotVipLevel, createdUserId, createdUsername, cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask<bool> ExistAsync(string playerCode, PlayerVipLevelCycleFormCycleType cycleType, DateOnly cycleOn, CancellationToken cancellationToken = default)
	=> await database.KeyExistsAsync(
		key: GetKey(playerCode, cycleType, cycleOn))
		.ConfigureAwait(false)
		|| await repository.ExistAsync(playerCode, cycleType, cycleOn, cancellationToken)
		.ConfigureAwait(false);

	public ValueTask<PlayerVipLevelCycleFormInfo?> GetAsync(Guid id, CancellationToken cancellationToken = default)
		=> repository.GetAsync(id, cancellationToken);

	public ValueTask PendingAsync(Guid id, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default)
		=> repository.PendingAsync(id, updatedUserId, updatedUsername, cancellationToken);

	public ValueTask RejectAsync(Guid id, string remark, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default)
		=> repository.RejectAsync(id, remark, updatedUserId, updatedUsername, cancellationToken);

	public IAsyncEnumerable<PlayerVipLevelCycleFormQueryInfo> QueryAsync(Guid brandId, DateTimeOffset updatedOnStart, DateTimeOffset updatedOnEnd, string playerCode, string username, IEnumerable<PlayerVipLevelCycleFormState> states, int offset = 0, int limit = 20, CancellationToken cancellationToken = default)
		=> repository.QueryAsync(brandId: brandId, updatedOnStart: updatedOnStart, updatedOnEnd: updatedOnEnd, playerCode: playerCode, username: username, states: states, offset: offset, limit: limit, cancellationToken: cancellationToken);

	public ValueTask<int> QueryCountAsync(Guid brandId, DateTimeOffset updatedOnStart, DateTimeOffset updatedOnEnd, string playerCode, string username, IEnumerable<PlayerVipLevelCycleFormState> states, CancellationToken cancellationToken = default)
		=> repository.QueryCountAsync(brandId: brandId, updatedOnStart: updatedOnStart, updatedOnEnd: updatedOnEnd, playerCode: playerCode, username: username, states: states, cancellationToken: cancellationToken);

	private static string GetKey(string playerCode, PlayerVipLevelCycleFormCycleType cycleType, DateOnly cycleOn)
				=> string.Concat(typeof(PlayerVipLevelCycleFormCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", typeof(PlayerVipLevelCycleFormCacheRepository).Name!.ToLower(), ":", playerCode, ":", cycleType.ToString(), ":", cycleOn);

	private async ValueTask<bool> SetAsync(string playerCode, PlayerVipLevelCycleFormCycleType cycleType, DateOnly cycleOn)
		=> await database.StringSetAsync(
			key: GetKey(playerCode, cycleType, cycleOn),
			value: JsonSerializer.Serialize(new
			{
				PlayerCode = playerCode,
				CycleType = cycleType,
				CycleOn = cycleOn,
				LogOn = DateTimeOffset.UtcNow
			}),
			expiry: expiry,
			when: When.NotExists)
		.ConfigureAwait(false);
}
