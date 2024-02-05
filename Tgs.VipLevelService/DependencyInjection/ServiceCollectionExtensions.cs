using Microsoft.EntityFrameworkCore;
using Tgs.VipLevelService;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddVipLevelServices(
		this IServiceCollection services,
		Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions,
		Action<RedisOptions, IServiceProvider> redisOptions)
		=> services
		.AddOptions<RedisOptions>()
		.Configure(redisOptions)
		.Services
		.AddSingleton<RedisConnectionManager>()
		.AddPlayerBucketClient((httpClientBuilder) =>
		httpClientBuilder.ConfigureHttpClient((sp, http) =>
			{
				var configuration = sp.GetRequiredService<IConfiguration>();
				http.BaseAddress = configuration.GetValue<Uri>("VipLevelConnectionStrings:PlayerBucketClientBaseUrl")!;
			}))
		.AddCashServiceClient((options, sp) =>
		{
			var configuration = sp.GetRequiredService<IConfiguration>();
			options.BaseUrl = configuration.GetValue<Uri>("VipLevelConnectionStrings:CashServiceClientBaseUrl")!;
		})
		.AddVipLevelCore()
		.AddVipLevelRepositories(
			dbContextOptions: dbContextOptions,
			redisDatabaseResolver: (sp) => sp.GetRequiredService<RedisConnectionManager>().Database);
}
