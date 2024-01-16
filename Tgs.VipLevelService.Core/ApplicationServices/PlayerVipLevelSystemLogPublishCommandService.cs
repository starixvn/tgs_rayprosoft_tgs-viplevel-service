using System.Text;

using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.Common;
using Tgs.VipLevelService.Common.MessageModels;
using Valhalla.MessageQueue;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PlayerVipLevelSystemLogPublishCommandService(
	ILogger<PlayerVipLevelSystemLogPublishCommandService> logger,
	IMessageSender messageSender) : ICommandService<PlayerVipLevelSystemLogPublishCommand>
{
	public async ValueTask ExecuteAsync(PlayerVipLevelSystemLogPublishCommand command, CancellationToken cancellationToken = default)
	{
		var message = new PlayerVipLevelLogSystemMessageModel(
			BrandId: command.BrandId,
			PlayerCode: command.PlayerCode,
			BeforeLevel: command.BeforeLevel,
			AfterLevel: command.AfterLevel,
			AutoState: command.AutoState);

		await messageSender.PublishAsync(
			subject: PlayerVipLevelLogNatsConsts.System,
			data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:This message has been published.({message})",
			DateTime.Now.ToString("u"),
			PlayerVipLevelLogNatsConsts.System,
			message);
	}
}
