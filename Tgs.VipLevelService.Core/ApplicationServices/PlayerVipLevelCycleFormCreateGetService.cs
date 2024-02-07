using Tgs.SerialNumberGenerator;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PlayerVipLevelCycleFormCreateGetService(
	ISerialNumberGenerator serialNumberGenerator,
	IPlayerVipLevelCycleFormRepository repository
	) : IGetService<PlayerVipLevelCycleFormCreateCommand, Guid>
{
	public async ValueTask<Guid> GetAsync(PlayerVipLevelCycleFormCreateCommand query, CancellationToken cancellationToken = default)
	{
		var id = await serialNumberGenerator.GenerateAsync(cancellationToken).ConfigureAwait(false);
		await repository.CreateAsync(
			id: id,
			brandId: query.BrandId,
			playerCode: query.PlayerCode,
			vipLevel: query.VipLevel,
			cycleType: query.CycleType,
			cycleOn: query.CycleOn,
			fromOn: query.FromOn,
			toOn: query.ToOn,
			amount: query.Amount,
			snapshotVipLevel: new PlayerVipLevelCycleFormSnapshotVipLevelInfo(
				VipLevelId: query.VipLevelInfo.Id,
				VipLevel: query.VipLevelInfo.Level,
				BrandId: query.VipLevelInfo.BrandId,
				QuarterlyBenefit: query.VipLevelInfo.QuarterlyBenefit),
			createdUserId: query.OperatorUserId,
			createdUsername: query.OperatorUsername,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return id;
	}
}
