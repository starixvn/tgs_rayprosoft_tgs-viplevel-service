using NSubstitute;
using Tgs.VipLevelService.ApplicationServices;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;

public class VipLevelByIdGetServiceUnitTests
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeVipLevelRepository = Substitute.For<IVipLevelRepository>();

		var sut = new VipLevelByIdGetService(fakeVipLevelRepository);

		var query = new VipLevelByIdQuery(
			BrandId: Guid.NewGuid(),
			Id: Guid.NewGuid());

		_ = await sut.GetAsync(query);

		_ = fakeVipLevelRepository
			.Received(1)
			.GetAsync(
			brandId: Arg.Is(query.BrandId),
			id: Arg.Is(query.Id),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
