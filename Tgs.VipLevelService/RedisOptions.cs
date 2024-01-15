namespace Tgs.VipLevelService;

public record RedisOptions
{
	public string ConnectionString { get; set; } = "localhost";
}
