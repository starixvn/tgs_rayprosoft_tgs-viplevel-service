using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;
public class PlayerVipLevelExpFormAutoCommandServiceUnitTest
{
	[Fact]
	public async Task AutoState_Is_True()
	{
		var fakeLog = NullLogger<PlayerVipLevelExpFormAutoCommandService>.Instance;
		var fakePlayerVipLevelExpFormRepository = Substitute.For<IPlayerVipLevelExpFormRepository>();
		var fakeService = Substitute.For<ICommandService<PlayerVipLevelExpFormApprovedCommand>>();

		var command = new PlayerVipLevelExpFormAutoCommand(
			Id: Guid.NewGuid());

		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";

		var form = new PlayerVipLevelExpFormInfo(
			Id: command.Id,
			BrandId: Guid.NewGuid(),
			PlayerCode: "4um1qf1p0v3z",
			VipLevel: 1,
			Type: Protos.PlayerVipLevelExpFormType.Turnover,
			AutoState: true,
			State: Protos.PlayerVipLevelExpFormState.Create,
			Sign: 1,
			Amount: 1,
			ActualAmount: 1,
			Remark: "test",
			CreatedOn: DateTimeOffset.UtcNow,
			CreatedUserId: operatorUserId,
			CreatedUsername: operatorUsername,
			UpdatedOn: DateTimeOffset.UtcNow,
			UpdatedUserId: operatorUserId,
			UpdatedUsername: operatorUsername);
		_ = fakePlayerVipLevelExpFormRepository.GetAsync(
			id: command.Id,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(form);

		var sut = new PlayerVipLevelExpFormAutoCommandService(
			fakeLog,
			fakePlayerVipLevelExpFormRepository,
			fakeService);

		await sut.ExecuteAsync(command);

		_ = fakeService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new PlayerVipLevelExpFormApprovedCommand(
				Id: form.Id,
				Amount: form.Amount,
				Remark: form.Remark,
				OperatorUserId: form.CreatedUserId,
				OperatorUsername: form.CreatedUsername)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelExpFormRepository
			.Received(0)
			.PendingAsync(
			id: Arg.Is(form.Id),
			updatedUserId: Arg.Is(form.CreatedUserId),
			updatedUsername: Arg.Is(form.CreatedUsername),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task AutoState_Is_False()
	{
		var fakeLog = NullLogger<PlayerVipLevelExpFormAutoCommandService>.Instance;
		var fakePlayerVipLevelExpFormRepository = Substitute.For<IPlayerVipLevelExpFormRepository>();
		var fakeService = Substitute.For<ICommandService<PlayerVipLevelExpFormApprovedCommand>>();

		var command = new PlayerVipLevelExpFormAutoCommand(
			Id: Guid.NewGuid());

		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";

		var form = new PlayerVipLevelExpFormInfo(
			Id: command.Id,
			BrandId: Guid.NewGuid(),
			PlayerCode: "4um1qf1p0v3z",
			VipLevel: 1,
			Type: Protos.PlayerVipLevelExpFormType.Turnover,
			AutoState: false,
			State: Protos.PlayerVipLevelExpFormState.Create,
			Sign: 1,
			Amount: 1,
			ActualAmount: 1,
			Remark: "test",
			CreatedOn: DateTimeOffset.UtcNow,
			CreatedUserId: operatorUserId,
			CreatedUsername: operatorUsername,
			UpdatedOn: DateTimeOffset.UtcNow,
			UpdatedUserId: operatorUserId,
			UpdatedUsername: operatorUsername);
		_ = fakePlayerVipLevelExpFormRepository.GetAsync(
			id: command.Id,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(form);

		var sut = new PlayerVipLevelExpFormAutoCommandService(
			fakeLog,
			fakePlayerVipLevelExpFormRepository,
			fakeService);

		await sut.ExecuteAsync(command);

		_ = fakeService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Is(new PlayerVipLevelExpFormApprovedCommand(
				Id: form.Id,
				Amount: form.Amount,
				Remark: form.Remark,
				OperatorUserId: form.CreatedUserId,
				OperatorUsername: form.CreatedUsername)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelExpFormRepository
			.Received(1)
			.PendingAsync(
			id: Arg.Is(form.Id),
			updatedUserId: Arg.Is(form.CreatedUserId),
			updatedUsername: Arg.Is(form.CreatedUsername),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_form_status_is_not_created()
	{
		var fakeLog = NullLogger<PlayerVipLevelExpFormAutoCommandService>.Instance;
		var fakePlayerVipLevelExpFormRepository = Substitute.For<IPlayerVipLevelExpFormRepository>();
		var fakeService = Substitute.For<ICommandService<PlayerVipLevelExpFormApprovedCommand>>();

		var command = new PlayerVipLevelExpFormAutoCommand(
			Id: Guid.NewGuid());

		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";

		var form = new PlayerVipLevelExpFormInfo(
			Id: command.Id,
			BrandId: Guid.NewGuid(),
			PlayerCode: "4um1qf1p0v3z",
			VipLevel: 1,
			Type: Protos.PlayerVipLevelExpFormType.Turnover,
			AutoState: true,
			State: Protos.PlayerVipLevelExpFormState.Approved,
			Sign: 1,
			Amount: 1,
			ActualAmount: 1,
			Remark: "test",
			CreatedOn: DateTimeOffset.UtcNow,
			CreatedUserId: operatorUserId,
			CreatedUsername: operatorUsername,
			UpdatedOn: DateTimeOffset.UtcNow,
			UpdatedUserId: operatorUserId,
			UpdatedUsername: operatorUsername);
		_ = fakePlayerVipLevelExpFormRepository.GetAsync(
			id: command.Id,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(form);

		var sut = new PlayerVipLevelExpFormAutoCommandService(
			fakeLog,
			fakePlayerVipLevelExpFormRepository,
			fakeService);

		await sut.ExecuteAsync(command);

		_ = fakeService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Is(new PlayerVipLevelExpFormApprovedCommand(
				Id: form.Id,
				Amount: form.Amount,
				Remark: form.Remark,
				OperatorUserId: form.CreatedUserId,
				OperatorUsername: form.CreatedUsername)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelExpFormRepository
			.Received(0)
			.PendingAsync(
			id: Arg.Is(form.Id),
			updatedUserId: Arg.Is(form.CreatedUserId),
			updatedUsername: Arg.Is(form.CreatedUsername),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_form_cannot_be_found()
	{
		var fakeLog = NullLogger<PlayerVipLevelExpFormAutoCommandService>.Instance;
		var fakePlayerVipLevelExpFormRepository = Substitute.For<IPlayerVipLevelExpFormRepository>();
		var fakeService = Substitute.For<ICommandService<PlayerVipLevelExpFormApprovedCommand>>();

		var command = new PlayerVipLevelExpFormAutoCommand(
			Id: Guid.NewGuid());

		_ = fakePlayerVipLevelExpFormRepository.GetAsync(
			id: command.Id,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns((PlayerVipLevelExpFormInfo?)null);

		var sut = new PlayerVipLevelExpFormAutoCommandService(
			fakeLog,
			fakePlayerVipLevelExpFormRepository,
			fakeService);

		_ = await Assert.ThrowsAsync<NullReferenceException>(() => sut.ExecuteAsync(command).AsTask());

		_ = fakeService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<PlayerVipLevelExpFormApprovedCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelExpFormRepository
			.Received(0)
			.PendingAsync(
			id: Arg.Any<Guid>(),
			updatedUserId: Arg.Any<Guid>(),
			updatedUsername: Arg.Any<string>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
