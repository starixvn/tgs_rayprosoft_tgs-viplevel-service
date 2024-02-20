using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using Tgs.VipLevels.Exp.Worker.Common;
using Tgs.VipLevels.Exp.Worker.Common.Protos;
using Valhalla.MessageQueue;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PlayerVipLevelExpFormApprovedCommandService(
	ILogger<PlayerVipLevelExpFormApprovedCommandService> logger,
	IPlayerVipLevelExpFormRepository playerVipLevelExpFormRepository,
	IMessageSender messageSender) : ICommandService<PlayerVipLevelExpFormApprovedCommand>
{
	public async ValueTask ExecuteAsync(PlayerVipLevelExpFormApprovedCommand command, CancellationToken cancellationToken = default)
	{
		var form = await playerVipLevelExpFormRepository.GetAsync(
			id: command.Id,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new NullReferenceException($"This form cannot be found.{command.Id}");

		if (form.State is Core.Protos.PlayerVipLevelExpFormState.Approved or Core.Protos.PlayerVipLevelExpFormState.Reject)
		{
			logger.LogWarning("LogOn:{logOn} Message:This form({id}) has been reviewed.({state})",
				DateTime.Now.ToString("u"),
				form.Id,
				form.State);
			return;
		}

		await playerVipLevelExpFormRepository.ApprovedAsync(
			id: form.Id,
			amount: command.Amount,
			remark: command.Remark,
			updatedUserId: command.OperatorUserId,
			updatedUsername: command.OperatorUsername,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		/*
		 * 發送通知
		 */
		await messageSender.PublishAsync(
			subject: VipLevelsExpWorkerNatsConst.ExpFormId,
			data: new VipLevelsExpFormMessage
			{
				FormId = form.Id,
			},
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
