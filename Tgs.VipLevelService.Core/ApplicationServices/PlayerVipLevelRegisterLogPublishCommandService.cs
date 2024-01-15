using System.Text;

using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.Common;
using Tgs.VipLevelService.Common.MessageModels;
using Valhalla.MessageQueue;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PlayerVipLevelRegisterLogPublishCommandService(
	ILogger<PlayerVipLevelRegisterLogPublishCommandService> logger,
	IMessageSender messageSender) : ICommandService<PlayerVipLevelRegisterLogPublishCommand>
{
	public async ValueTask ExecuteAsync(PlayerVipLevelRegisterLogPublishCommand command, CancellationToken cancellationToken = default)
	{
		var message = new PlayerVipLevelLogRegisterMessageModel(
			BrandId: command.BrandId,
			PlayerCode: command.PlayerCode,
			AfterLevel: command.AfterLevel,
			AfterAutoState: command.AfterAutoState);

		await messageSender.PublishAsync(
			subject: PlayerVipLevelLogNatsConsts.Register,
			data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:This message has been published.({message})",
			DateTime.Now.ToString("u"),
			PlayerVipLevelLogNatsConsts.Register,
			message);
	}
}
