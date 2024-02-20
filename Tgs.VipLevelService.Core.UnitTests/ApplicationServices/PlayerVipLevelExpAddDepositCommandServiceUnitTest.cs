using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.Calculate.Clients;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;

public class PlayerVipLevelExpAddDepositCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeLog = NullLogger<PlayerVipLevelExpAddDepositCommandService>.Instance;
		var fakeOptions = Options.Create(new VipLevelPolicySettingsOptions
		{
			PlayerVipLevelExpDepositRetryCount = 1,
			PlayerVipLevelExpDepositRetryAttemptSec = 1,
			PlayerVipLevelExpTurnoverRetryCount = 1,
			PlayerVipLevelExpTurnoverRetryAttemptSec = 1
		});
		var fakeCryptographyUtensil = Substitute.For<ICryptographyUtensil>();
		var fakeCalculateClient = Substitute.For<ICalculateClient>();
		var fakePlayerVipLevelExpRepository = Substitute.For<IPlayerVipLevelExpRepository>();
		var fakePlayerVipLevelExpDepositRecordRepository = Substitute.For<IPlayerVipLevelExpDepositRecordRepository>();
		var fakePlayerVipLevelExpFixesService = Substitute.For<IGetService<PlayerVipLevelExpFixesQuery, PlayerVipLevelExpInfo>>();
		var fakePlayerVipLevelCheckService = Substitute.For<ICommandService<PlayerVipLevelCheckCommand>>();

		var command = new PlayerVipLevelExpAddDepositCommand(
			PlayerCode: "4ucmqplxb029",
			FormId: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
			Amount: 100m);

		var playerVipLevelExpInfo = new PlayerVipLevelExpInfo(
			PlayerCode: command.PlayerCode,
			AccumDepositId: Guid.NewGuid(),
			AccumDeposit: 100,
			AccumTurnoverId: Guid.NewGuid(),
			AccumTurnover: 200);
		_ = fakePlayerVipLevelExpRepository.GetAsync(
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipLevelExpInfo);

		var data = string.Concat(
			"Type", "=", PlayerVipLevelExpDepositRecordType.Normal, "&",
			  "PlayerCode", "=", playerVipLevelExpInfo.PlayerCode, "&",
			  "Form", "=", command.FormId);
		var hash = Guid.NewGuid().ToString("N");
		_ = fakeCryptographyUtensil.Sha512(
			source: Arg.Is(data))
			.Returns(hash);

		_ = fakePlayerVipLevelExpDepositRecordRepository.ExistAsync(
			Arg.Is(hash),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(false);

		var exchange = new ExchangeResult(playerVipLevelExpInfo.AccumDepositId, 200);
		_ = fakeCalculateClient.ExchangeAsync(
			calculateId: Arg.Is(playerVipLevelExpInfo.AccumDepositId),
			change: Arg.Is(command.Amount),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(exchange);

		var sut = new PlayerVipLevelExpAddDepositCommandService(
			fakeLog,
			fakeOptions,
			fakeCryptographyUtensil,
			fakeCalculateClient,
			fakePlayerVipLevelExpRepository,
			fakePlayerVipLevelExpDepositRecordRepository,
			fakePlayerVipLevelExpFixesService,
			fakePlayerVipLevelCheckService);

		await sut.ExecuteAsync(command);

		_ = fakePlayerVipLevelExpFixesService
			.Received(0)
			.GetAsync(
			query: Arg.Is(new PlayerVipLevelExpFixesQuery(
				PlayerCode: playerVipLevelExpInfo.PlayerCode)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelExpRepository
			.Received(1)
			.UpdateDepositAsync(
			playerCode: Arg.Is(playerVipLevelExpInfo.PlayerCode),
			accumDeposit: Arg.Is(exchange.PostBalance),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelExpDepositRecordRepository
			.Received(1)
			.AddAsync(
			hash: Arg.Is(hash),
			 playerCode: Arg.Is(playerVipLevelExpInfo.PlayerCode),
			 type: Arg.Is(PlayerVipLevelExpDepositRecordType.Normal),
			 sign: Arg.Is(1),
			 amount: Arg.Is(command.Amount),
			 balance: Arg.Any<decimal>(),
			 additionalData: Arg.Any<JsonNode>(),
			 remark: Arg.Any<string>(),
			 createdOn: Arg.Any<DateTimeOffset>(),
			 createdUserId: Arg.Any<Guid>(),
			 createdUsername: Arg.Any<string>(),
			 cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelCheckService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new PlayerVipLevelCheckCommand(
				PlayerCode: playerVipLevelExpInfo.PlayerCode)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task RetryPolicy_HttpRequestException()
	{
		var fakeLog = NullLogger<PlayerVipLevelExpAddDepositCommandService>.Instance;
		var fakeOptions = Options.Create(new VipLevelPolicySettingsOptions
		{
			PlayerVipLevelExpDepositRetryCount = 1,
			PlayerVipLevelExpDepositRetryAttemptSec = 1,
			PlayerVipLevelExpTurnoverRetryCount = 1,
			PlayerVipLevelExpTurnoverRetryAttemptSec = 1
		});
		var fakeCryptographyUtensil = Substitute.For<ICryptographyUtensil>();
		var fakeCalculateClient = Substitute.For<ICalculateClient>();
		var fakePlayerVipLevelExpRepository = Substitute.For<IPlayerVipLevelExpRepository>();
		var fakePlayerVipLevelExpDepositRecordRepository = Substitute.For<IPlayerVipLevelExpDepositRecordRepository>();
		var fakePlayerVipLevelExpFixesService = Substitute.For<IGetService<PlayerVipLevelExpFixesQuery, PlayerVipLevelExpInfo>>();
		var fakePlayerVipLevelCheckService = Substitute.For<ICommandService<PlayerVipLevelCheckCommand>>();

		var command = new PlayerVipLevelExpAddDepositCommand(
			PlayerCode: "4ucmqplxb029",
			FormId: DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
			Amount: 100m);

		var playerVipLevelExpInfo = new PlayerVipLevelExpInfo(
			PlayerCode: command.PlayerCode,
			AccumDepositId: Guid.NewGuid(),
			AccumDeposit: 100,
			AccumTurnoverId: Guid.NewGuid(),
			AccumTurnover: 200);
		_ = fakePlayerVipLevelExpRepository.GetAsync(
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipLevelExpInfo);

		var data = string.Concat(
			"Type", "=", PlayerVipLevelExpDepositRecordType.Normal, "&",
			  "PlayerCode", "=", playerVipLevelExpInfo.PlayerCode, "&",
			  "Form", "=", command.FormId);
		var hash = Guid.NewGuid().ToString("N");
		_ = fakeCryptographyUtensil.Sha512(
			source: Arg.Is(data))
			.Returns(hash);

		_ = fakePlayerVipLevelExpDepositRecordRepository.ExistAsync(
			Arg.Is(hash),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(false);

		var exchange = new ExchangeResult(playerVipLevelExpInfo.AccumDepositId, 200);
		_ = fakeCalculateClient.ExchangeAsync(
			calculateId: Arg.Is(playerVipLevelExpInfo.AccumDepositId),
			change: Arg.Is(command.Amount),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromException<ExchangeResult>(new HttpRequestException()), ValueTask.FromResult(exchange));

		var sut = new PlayerVipLevelExpAddDepositCommandService(
			fakeLog,
			fakeOptions,
			fakeCryptographyUtensil,
			fakeCalculateClient,
			fakePlayerVipLevelExpRepository,
			fakePlayerVipLevelExpDepositRecordRepository,
			fakePlayerVipLevelExpFixesService,
			fakePlayerVipLevelCheckService);

		await sut.ExecuteAsync(command);

		_ = fakePlayerVipLevelExpFixesService
			.Received(1)
			.GetAsync(
			query: Arg.Is(new PlayerVipLevelExpFixesQuery(
				PlayerCode: playerVipLevelExpInfo.PlayerCode)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelExpRepository
			.Received(1)
			.UpdateDepositAsync(
			playerCode: Arg.Is(playerVipLevelExpInfo.PlayerCode),
			accumDeposit: Arg.Is(exchange.PostBalance),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelExpDepositRecordRepository
			.Received(1)
			.AddAsync(
			hash: Arg.Is(hash),
			 playerCode: Arg.Is(playerVipLevelExpInfo.PlayerCode),
			 type: Arg.Is(PlayerVipLevelExpDepositRecordType.Normal),
			 sign: Arg.Is(1),
			 amount: Arg.Is(command.Amount),
			 balance: Arg.Any<decimal>(),
			 additionalData: Arg.Any<JsonNode>(),
			 remark: Arg.Any<string>(),
			 createdOn: Arg.Any<DateTimeOffset>(),
			 createdUserId: Arg.Any<Guid>(),
			 createdUsername: Arg.Any<string>(),
			 cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelCheckService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new PlayerVipLevelCheckCommand(
				PlayerCode: playerVipLevelExpInfo.PlayerCode)),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
