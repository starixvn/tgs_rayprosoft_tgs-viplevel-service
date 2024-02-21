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
public class PlayerVipLevelCycleFormAutoCommandServiceUnitTest
{
	[Fact]
	public async Task AutoState_Is_True()
	{
		var fakeLog = NullLogger<PlayerVipLevelCycleFormAutoCommandService>.Instance;
		var fakePlayerVipLevelCycleFormRepository = Substitute.For<IPlayerVipLevelCycleFormRepository>();
		var fakeService = Substitute.For<ICommandService<PlayerVipLevelCycleFormApprovedCommand>>();

		var command = new PlayerVipLevelCycleFormAutoCommand(
			Id: Guid.NewGuid());

		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";

		var form = new PlayerVipLevelCycleFormInfo(
			Id: command.Id,
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			VipLevel: 1,
			CycleType: Protos.PlayerVipLevelCycleFormCycleType.Season,
			CycleOn: DateOnly.Parse("2024-01-01"),
			FromOn: DateOnly.Parse("2023-10-01"),
			ToOn: DateOnly.Parse("2024-01-01"),
			AutoState: true,
			State: Protos.PlayerVipLevelCycleFormState.Create,
			Amount: 99m,
			ActualAmount: 0.0m,
			Remark: string.Empty,
			CreatedOn: DateTimeOffset.Now,
			CreatedUserId: operatorUserId,
			CreatedUsername: operatorUsername,
			UpdatedOn: DateTimeOffset.Now,
			UpdatedUserId: operatorUserId,
			UpdatedUsername: operatorUsername);
		_ = fakePlayerVipLevelCycleFormRepository.GetAsync(
			id: command.Id,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(form);

		var sut = new PlayerVipLevelCycleFormAutoCommandService(
			fakeLog,
			fakePlayerVipLevelCycleFormRepository,
			fakeService);

		await sut.ExecuteAsync(command);

		_ = fakeService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new PlayerVipLevelCycleFormApprovedCommand(
				Id: form.Id,
				Amount: form.Amount,
				Remark: form.Remark,
				OperatorUserId: form.CreatedUserId,
				OperatorUsername: form.CreatedUsername)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelCycleFormRepository
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
		var fakeLog = NullLogger<PlayerVipLevelCycleFormAutoCommandService>.Instance;
		var fakePlayerVipLevelCycleFormRepository = Substitute.For<IPlayerVipLevelCycleFormRepository>();
		var fakeService = Substitute.For<ICommandService<PlayerVipLevelCycleFormApprovedCommand>>();

		var command = new PlayerVipLevelCycleFormAutoCommand(
			Id: Guid.NewGuid());

		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";

		var form = new PlayerVipLevelCycleFormInfo(
			Id: command.Id,
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			VipLevel: 1,
			CycleType: Protos.PlayerVipLevelCycleFormCycleType.Season,
			CycleOn: DateOnly.Parse("2024-01-01"),
			FromOn: DateOnly.Parse("2023-10-01"),
			ToOn: DateOnly.Parse("2024-01-01"),
			AutoState: false,
			State: Protos.PlayerVipLevelCycleFormState.Create,
			Amount: 99m,
			ActualAmount: 0.0m,
			Remark: string.Empty,
			CreatedOn: DateTimeOffset.Now,
			CreatedUserId: operatorUserId,
			CreatedUsername: operatorUsername,
			UpdatedOn: DateTimeOffset.Now,
			UpdatedUserId: operatorUserId,
			UpdatedUsername: operatorUsername);
		_ = fakePlayerVipLevelCycleFormRepository.GetAsync(
			id: command.Id,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(form);

		var sut = new PlayerVipLevelCycleFormAutoCommandService(
			fakeLog,
			fakePlayerVipLevelCycleFormRepository,
			fakeService);

		await sut.ExecuteAsync(command);

		_ = fakeService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Is(new PlayerVipLevelCycleFormApprovedCommand(
				Id: form.Id,
				Amount: form.Amount,
				Remark: form.Remark,
				OperatorUserId: form.CreatedUserId,
				OperatorUsername: form.CreatedUsername)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelCycleFormRepository
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
		var fakeLog = NullLogger<PlayerVipLevelCycleFormAutoCommandService>.Instance;
		var fakePlayerVipLevelCycleFormRepository = Substitute.For<IPlayerVipLevelCycleFormRepository>();
		var fakeService = Substitute.For<ICommandService<PlayerVipLevelCycleFormApprovedCommand>>();

		var command = new PlayerVipLevelCycleFormAutoCommand(
			Id: Guid.NewGuid());

		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";

		var form = new PlayerVipLevelCycleFormInfo(
			Id: command.Id,
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			VipLevel: 1,
			CycleType: Protos.PlayerVipLevelCycleFormCycleType.Season,
			CycleOn: DateOnly.Parse("2024-01-01"),
			FromOn: DateOnly.Parse("2023-10-01"),
			ToOn: DateOnly.Parse("2024-01-01"),
			AutoState: true,
			State: Protos.PlayerVipLevelCycleFormState.Approved,
			Amount: 99m,
			ActualAmount: 0.0m,
			Remark: string.Empty,
			CreatedOn: DateTimeOffset.Now,
			CreatedUserId: operatorUserId,
			CreatedUsername: operatorUsername,
			UpdatedOn: DateTimeOffset.Now,
			UpdatedUserId: operatorUserId,
			UpdatedUsername: operatorUsername);
		_ = fakePlayerVipLevelCycleFormRepository.GetAsync(
			id: command.Id,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(form);
		_ = fakePlayerVipLevelCycleFormRepository.GetAsync(
			id: command.Id,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(form);

		var sut = new PlayerVipLevelCycleFormAutoCommandService(
			fakeLog,
			fakePlayerVipLevelCycleFormRepository,
			fakeService);

		await sut.ExecuteAsync(command);

		_ = fakeService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Is(new PlayerVipLevelCycleFormApprovedCommand(
				Id: form.Id,
				Amount: form.Amount,
				Remark: form.Remark,
				OperatorUserId: form.CreatedUserId,
				OperatorUsername: form.CreatedUsername)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelCycleFormRepository
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
		var fakeLog = NullLogger<PlayerVipLevelCycleFormAutoCommandService>.Instance;
		var fakePlayerVipLevelCycleFormRepository = Substitute.For<IPlayerVipLevelCycleFormRepository>();
		var fakeService = Substitute.For<ICommandService<PlayerVipLevelCycleFormApprovedCommand>>();

		var command = new PlayerVipLevelCycleFormAutoCommand(
			Id: Guid.NewGuid());

		_ = fakePlayerVipLevelCycleFormRepository.GetAsync(
			id: command.Id,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns((PlayerVipLevelCycleFormInfo?)null);

		var sut = new PlayerVipLevelCycleFormAutoCommandService(
			fakeLog,
			fakePlayerVipLevelCycleFormRepository,
			fakeService);

		_ = await Assert.ThrowsAsync<NullReferenceException>(() => sut.ExecuteAsync(command).AsTask());

		_ = fakeService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<PlayerVipLevelCycleFormApprovedCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelCycleFormRepository
			.Received(0)
			.PendingAsync(
			id: Arg.Any<Guid>(),
			updatedUserId: Arg.Any<Guid>(),
			updatedUsername: Arg.Any<string>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
