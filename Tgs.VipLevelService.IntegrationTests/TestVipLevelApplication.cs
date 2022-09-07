using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using Tgs.VipLevelService.Entities;

namespace Tgs.VipLevelService.IntegrationTests;

internal static class TestVipLevelApplication
{
	private static readonly WebApplicationFactory<Program> _Application = new();

	public static WebApplicationFactory<Program> Factory => _Application
		.WithWebHostBuilder(builder => builder
			.ConfigureServices(services => services
				.AddSingleton<IConfigureOptions<OtlpExporterOptions>>(
					sp => new ConfigureNamedOptions<OtlpExporterOptions>(
						Options.DefaultName,
						exporterOptions => exporterOptions.TimeoutMilliseconds = 0))
				.Replace(ServiceDescriptor.Singleton(sp =>
				{
					var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
					_ = dbContextOptionsBuilder
						.UseInMemoryDatabase(Guid.NewGuid().ToString())
						.ConfigureWarnings(x => x.Ignore(
							InMemoryEventId.TransactionIgnoredWarning));

					return new TgsContext(dbContextOptionsBuilder.Options);
				}))));
}
