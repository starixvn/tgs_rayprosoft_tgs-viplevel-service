using NSubstitute;
using Tgs.VipLevelService.ApplicationServices;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;

public class VipLevelSingleByIdGetServiceUnitTests
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeVipLevelRepository = Substitute.For<IVipLevelRepository>();

		var sut = new VipLevelSingleByIdGetService(fakeVipLevelRepository);

		var query = new VipLevelSingleByIdQuery(
			Id: Guid.NewGuid());

		_ = await sut.GetAsync(query);

		_ = fakeVipLevelRepository
			.Received(1)
			.GetAsync(
			id: Arg.Is(query.Id),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
