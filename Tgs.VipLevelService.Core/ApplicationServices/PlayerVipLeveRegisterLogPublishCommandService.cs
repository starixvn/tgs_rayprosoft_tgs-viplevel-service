using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.Common;
using Valhalla.MessageQueue;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PlayerVipLeveRegisterLogPublishCommandService(
	ILogger<PlayerVipLeveRegisterLogPublishCommandService> logger,
	IMessageSender messageSender) : ICommandService<PlayerVipLeveRegisterLogPublishCommand>
{
	public async ValueTask ExecuteAsync(PlayerVipLeveRegisterLogPublishCommand command, CancellationToken cancellationToken = default)
	{
		var message = new PlayerVipLevelLogRegisterMessage(
			BrandId: command.BrandId,
			PlayerCode: command.PlayerCode,
			AfterLevel: command.AfterLevel,
			AfterAutoState: command.AfterAutoState);

		await messageSender.PublishAsync(
			subject: PlayerVipLevelLogNatsConsts.Register,
			data: message,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:This message has been published.({message})",
			DateTime.Now.ToString("u"),
			PlayerVipLevelLogNatsConsts.Register,
			message);
	}
}
