using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.Core.Protos;
using Valhalla.MessageQueue;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PlayerVipLevelManualLogPublishCommandService(
	ILogger<PlayerVipLevelManualLogPublishCommandService> logger,
	IMessageSender messageSender) : ICommandService<PlayerVipLevelManualLogPublishCommand>
{
	public async ValueTask ExecuteAsync(PlayerVipLevelManualLogPublishCommand command, CancellationToken cancellationToken = default)
	{
		var request = new PlayerVipLevelLogManualRequest
		{
			BrandId = command.BrandId,
			PlayerCode = command.PlayerCode,
			BeforeLevel = command.BeforeLevel.GetValueOrDefault(0),
			BeforeAutoState = command.BeforeAutoState ?? Core.Protos.PlayerVipLevelAutoState.Enable,
			AfterLevel = command.AfterLevel,
			AfterAutoState = command.AfterAutoState,
			CreatedUserId = command.CreatedUserId,
			CreatedUsername = command.CreatedUsername,
			Remark = command.Remark
		};

		await messageSender.PublishAsync(
			subject: PlayerVipLevelLogNatsConsts.Manual,
			data: request.ToByteArray(),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		logger.LogInformation("LogOn:{logOn} Subject:{subject} Message:This message has been published.({message})",
			DateTime.Now.ToString("u"),
			PlayerVipLevelLogNatsConsts.Manual,
			request);
	}
}
