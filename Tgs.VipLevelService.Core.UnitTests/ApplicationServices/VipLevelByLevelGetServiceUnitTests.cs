using NSubstitute;
using Tgs.VipLevelService.ApplicationServices;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;

public class VipLevelByLevelGetServiceUnitTests
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeVipLevelRepository = Substitute.For<IVipLevelRepository>();

		var sut = new VipLevelByLevelGetService(fakeVipLevelRepository);

		var query = new VipLevelByLevelQuery(
			BrandId: Guid.NewGuid(),
			Level: 1);

		_ = await sut.GetAsync(query);

		_ = fakeVipLevelRepository
			.Received(1)
			.GetAsync(
			brandId: Arg.Is(query.BrandId),
			level: Arg.Is(query.Level),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
