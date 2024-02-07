using System.Text.Json;
using StackExchange.Redis;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Repositories;

internal class PlayerVipLevelBirthdayFormCacheRepository(
	IPlayerVipLevelBirthdayFormRepository repository,
	IDatabase database,
	TimeSpan expiry) : IPlayerVipLevelBirthdayFormRepository
{
	public ValueTask ApprovedAsync(Guid id, decimal amount, string remark, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default)
		=> repository.ApprovedAsync(id, amount, remark, updatedUserId, updatedUsername, cancellationToken);

	public async ValueTask CreateAsync(Guid id, Guid brandId, string playerCode, int vipLevel, DateOnly birthday, decimal amount, PlayerVipLevelBirthdaySnapshotVipLevelInfo snapshotVipLevel, Guid createdUserId, string createdUsername, CancellationToken cancellationToken = default)
	{
		await repository.CreateAsync(
			id, brandId, playerCode, vipLevel, birthday, amount, snapshotVipLevel, createdUserId, createdUsername, cancellationToken)
			.ConfigureAwait(false);
		await SetAsync(playerCode, birthday).ConfigureAwait(false);
	}

	public async ValueTask<bool> ExistAsync(string playerCode, DateOnly birthday, CancellationToken cancellationToken = default)
	=> await database.KeyExistsAsync(
		key: GetKey(playerCode, birthday))
		.ConfigureAwait(false)
		|| await repository.ExistAsync(playerCode, birthday, cancellationToken)
		.ConfigureAwait(false);

	public ValueTask<PlayerVipLevelBirthdayFormInfo?> GetAsync(Guid id, CancellationToken cancellationToken = default)
		=> repository.GetAsync(id, cancellationToken);

	public ValueTask PendingAsync(Guid id, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default)
		=> repository.PendingAsync(id, updatedUserId, updatedUsername, cancellationToken);

	public ValueTask RejectAsync(Guid id, string remark, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default)
		=> repository.RejectAsync(id, remark, updatedUserId, updatedUsername, cancellationToken);

	private static string GetKey(string playerCode, DateOnly birthday)
		=> string.Concat(typeof(PlayerVipLevelBirthdayFormCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", typeof(PlayerVipLevelBirthdayFormCacheRepository).Name!.ToLower(), ":", playerCode, ":", birthday.Year);

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
