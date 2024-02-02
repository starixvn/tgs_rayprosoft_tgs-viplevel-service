using Tgs.ServiceUtility;

namespace Tgs.VipLevelService.ApplicationServices;

internal class VipLevelSnapshotCurrentSnapshotIdGetService(
	IVipLevelSnapshotRepository vipLevelSnapshotRepository,
	ICommandService<VipLevelSnapshotCreateCommand> service) : IGetService<VipLevelSnapshotCurrentSnapshotIdQuery, Guid>
{
	public async ValueTask<Guid> GetAsync(VipLevelSnapshotCurrentSnapshotIdQuery query, CancellationToken cancellationToken = default)
	{
		var snapshotId = await vipLevelSnapshotRepository.GetCurrentSnapshotIdAsync(
			query.BrandId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!snapshotId.HasValue)
		{
			var command = new VipLevelSnapshotCreateCommand(
				BrandId: query.BrandId,
				EffectiveOn: DateTimeOffset.UtcNow,
				SnapshotUserId: Guid.Empty,
				SnapshotUsername: "admin.enabled");
			await service.ExecuteAsync(
				command: command,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			snapshotId = await vipLevelSnapshotRepository.GetCurrentSnapshotIdAsync(
				query.BrandId,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}

		return snapshotId.Value;
	}
}
