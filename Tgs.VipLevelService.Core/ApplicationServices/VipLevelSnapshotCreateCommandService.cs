using Microsoft.Extensions.Logging;
using Tgs.SerialNumberGenerator;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;
internal class VipLevelSnapshotCreateCommandService(
	ISerialNumberGenerator serialNumberGenerator,
	IVipLevelRepository vipLevelRepository,
	IVipLevelSnapshotRepository vipLevelSnapshotRepository) : ICommandService<VipLevelSnapshotCreateCommand>
{
	public async ValueTask ExecuteAsync(VipLevelSnapshotCreateCommand command, CancellationToken cancellationToken = default)
	{
		var vipLevelInfos = await vipLevelRepository.QueryAsync(
			brandId: command.BrandId,
			cancellationToken: cancellationToken)
			.ToArrayAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? Enumerable.Empty<VipLevelInfo>();

		if (!vipLevelInfos.Any())
		{
			throw new NullReferenceException($"The VipLevl setting cannot be found for this brand.{command.BrandId}");
		}

		var id = await serialNumberGenerator.GenerateAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await vipLevelSnapshotRepository.CreateAsync(
			id: id,
			brandId: command.BrandId,
			data: vipLevelInfos.OrderBy(keySelector: vls => vls.Level),
			effectiveOn: command.EffectiveOn,
			snapshotUserId: command.SnapshotUserId,
			snapshotUsername: command.SnapshotUsername,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
