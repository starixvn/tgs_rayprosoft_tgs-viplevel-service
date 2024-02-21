using Tgs.SerialNumberGenerator;
using Tgs.ServiceUtility;
using Tgs.VipLevels.Exp.Worker.Common.Protos;
using Tgs.VipLevelService.Models;
using Valhalla.MessageQueue;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PlayerVipLevelBirthdayFormCreateGetService(
	ISerialNumberGenerator serialNumberGenerator,
	IPlayerVipLevelBirthdayFormRepository repository,
	IMessageSender messageSender) : IGetService<PlayerVipLevelBirthdayFormCreateCommand, Guid>
{
	public async ValueTask<Guid> GetAsync(PlayerVipLevelBirthdayFormCreateCommand query, CancellationToken cancellationToken = default)
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
			birthday: query.Birthday,
			autoState: autoState,
			amount: query.Amount,
			snapshotVipLevel: new PlayerVipLevelBirthdaySnapshotVipLevelInfo(
				VipLevelId: query.VipLevelInfo.Id,
				VipLevel: query.VipLevelInfo.Level,
				BrandId: query.VipLevelInfo.BrandId,
				BirthBenefit: query.VipLevelInfo.BirthBenefit),
			createdUserId: query.OperatorUserId,
			createdUsername: query.OperatorUsername,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		/*
		 * 發送通知
		 */
		await messageSender.PublishAsync(
			subject: VipLevelServiceNatsConsts.PlayerVipLevelBirthdayFormAuto,
			data: new VipLevelsExpFormMessage
			{
				FormId = id,
			},
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return id;
	}
}
