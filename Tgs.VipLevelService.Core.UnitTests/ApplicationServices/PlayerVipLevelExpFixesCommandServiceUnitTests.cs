using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.VipLevels.Exp.Worker.Common;
using Tgs.VipLevels.Exp.Worker.Common.Protos;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Models;
using Valhalla.MessageQueue;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;
public class PlayerVipLevelExpFixesCommandServiceUnitTests
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeLog = NullLogger<PlayerVipLevelExpFixesCommandService>.Instance;
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new PlayerVipLevelExpFixesCommand(
			PlayerCode: "4ucmqplxb029");

		var service = new PlayerVipLevelExpFixesCommandService(
			fakeLog,
			fakeMessageSender);

		await service.ExecuteAsync(command);

		var message = new VipLevelsExpCreateMessage
		{
			PlayerCode = command.PlayerCode
		};
		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(VipLevelsExpWorkerNatsConst.Create),
			data: Arg.Is(message),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
