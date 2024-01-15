using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.Common;
using Valhalla.MessageQueue;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PlayerVipLevelManualLogPublishCommandService(
	ILogger<PlayerVipLevelManualLogPublishCommandService> logger,
	IMessageSender messageSender) : ICommandService<PlayerVipLevelManualLogPublishCommand>
{
	public async ValueTask ExecuteAsync(PlayerVipLevelManualLogPublishCommand command, CancellationToken cancellationToken = default)
	{
		var message = new PlayerVipLevelLogManualMessage(
			BrandId: command.BrandId,
			PlayerCode: command.PlayerCode,
			BeforeLevel: command.BeforeLevel,
			BeforeAutoState: command.BeforeAutoState,
			AfterLevel: command.AfterLevel,
			AfterAutoState: command.AfterAutoState,
			CreatedUserId: command.CreatedUserId,
			CreatedUsername: command.CreatedUsername);

		await messageSender.PublishAsync(
			subject: PlayerVipLevelLogNatsConsts.Manual,
			data: message,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:This message has been published.({message})",
			DateTime.Now.ToString("u"),
			PlayerVipLevelLogNatsConsts.Manual,
			message);
	}
}
