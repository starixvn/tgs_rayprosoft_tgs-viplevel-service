using System.Text.Json.Nodes;

namespace Tgs.VipLevelService;

public interface ICryptographyUtensil
{
	string Sha512(string source);
}
