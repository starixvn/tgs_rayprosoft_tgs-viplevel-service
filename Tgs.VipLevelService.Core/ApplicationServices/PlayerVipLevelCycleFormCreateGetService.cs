using Tgs.SerialNumberGenerator;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Models;
using Valhalla.MessageQueue;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PlayerVipLevelCycleFormCreateGetService(
	ISerialNumberGenerator serialNumberGenerator,
	IPlayerVipLevelCycleFormRepository repository,
	IMessageSender messageSender) : IGetService<PlayerVipLevelCycleFormCreateCommand, Guid>
{
	public async ValueTask<Guid> GetAsync(PlayerVipLevelCycleFormCreateCommand query, CancellationToken cancellationToken = default)
	{
		var id = await serialNumberGenerator.GenerateAsync(cancellationToken).ConfigureAwait(false);

		/*
		 * 自動/手動
		 */
		var autoState = true;

		await repository.CreateAsync(
			id: id,
			brandId: query.BrandId,
			playerCode: query.PlayerCode,
			vipLevel: query.VipLevel,
			cycleType: query.CycleType,
			cycleOn: query.CycleOn,
			fromOn: query.FromOn,
			toOn: query.ToOn,
			autoState: autoState,
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

		/*
		 * 發送通知
		 */
		await messageSender.PublishAsync(
			subject: VipLevelServiceNatsConsts.PlayerVipLevelCycleFormAuto,
			data: new VipLevelFormRequest
			{
				FormId = id,
			},
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return id;
	}
}
