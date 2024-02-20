using Tgs.SerialNumberGenerator;
using Tgs.ServiceUtility;
using Tgs.VipLevels.Exp.Worker.Common.Protos;
using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Models;
using Valhalla.MessageQueue;

namespace Tgs.VipLevelService.ApplicationServices;
internal class PlayerVipLevelExpFormCreateTurnoverCommandService(
	ISerialNumberGenerator serialNumberGenerator,
	IPlayerVipLevelExpFormRepository repository,
	IMessageSender messageSender) : IGetService<PlayerVipLevelExpFormCreateTurnoverCommand, Guid>
{
	public async ValueTask<Guid> GetAsync(PlayerVipLevelExpFormCreateTurnoverCommand query, CancellationToken cancellationToken = default)
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
			type: PlayerVipLevelExpFormType.Turnover,
			autoState: autoState,
			sign: query.Sign,
			amount: query.Amount,
			snapshotExp: new PlayerVipLevelExpFormSnapshotExpInfo(
				PlayerCode: query.PlayerVipLevel.PlayerCode,
				AccumDepositId: query.PlayerVipLevel.AccumDepositId,
				AccumDeposit: query.PlayerVipLevel.AccumDeposit,
				AccumTurnoverId: query.PlayerVipLevel.AccumTurnoverId,
				AccumTurnover: query.PlayerVipLevel.AccumTurnover),
			remark: query.Remark,
			createdUserId: query.OperatorUserId,
			createdUsername: query.OperatorUsername,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		/*
		 * 發送通知
		 */
		await messageSender.PublishAsync(
			subject: VipLevelServiceNatsConsts.PlayerVipLevelExpFormAuto,
			data: new VipLevelsExpFormMessage
			{
				FormId = id,
			},
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return id;
	}
}
