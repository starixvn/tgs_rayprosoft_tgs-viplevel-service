using System.Text.Json;
using System.Text.Json.Nodes;
using StackExchange.Redis;
using Tgs.VipLevelService.Core.Protos;

namespace Tgs.VipLevelService.Repositories;

internal class PlayerVipLevelExpDepositRecordCachRepository(
	IPlayerVipLevelExpDepositRecordRepository repository,
	IDatabase database,
	TimeSpan expiry) : IPlayerVipLevelExpDepositRecordRepository
{
	public async ValueTask AddAsync(string hash, string playerCode, PlayerVipLevelExpDepositRecordType type, int sign, decimal amount, decimal balance, JsonNode additionalData, string remark, DateTimeOffset createdOn, Guid createdUserId, string createdUsername, CancellationToken cancellationToken = default)
	{
		await repository.AddAsync(
			hash: hash,
			playerCode: playerCode,
			type: type,
			sign: sign,
			amount: amount,
			balance: balance,
			additionalData: additionalData,
			remark: remark,
			createdOn: createdOn,
			createdUserId: createdUserId,
			createdUsername: createdUsername,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await SetAsync(hash).ConfigureAwait(false);
	}

	public async ValueTask<bool> ExistAsync(string hash, CancellationToken cancellationToken = default)
	=> await database.KeyExistsAsync(
		key: GetKey(hash))
		.ConfigureAwait(false)
		|| await repository.ExistAsync(hash, cancellationToken)
		.ConfigureAwait(false);

	private static string GetKey(string hash)
		=> string.Concat(typeof(PlayerVipLevelExpDepositRecordCachRepository).Namespace!.Replace(".", "-").ToLower(), ":", typeof(PlayerVipLevelExpDepositRecordCachRepository).Name!.ToLower(), ":", hash);

	private async ValueTask<bool> SetAsync(string hash)
		=> await database.StringSetAsync(
			key: GetKey(hash),
			value: JsonSerializer.Serialize(new
			{
				Hash = hash,
				LogOn = DateTimeOffset.UtcNow
			}),
			expiry: expiry)
		.ConfigureAwait(false);
}
