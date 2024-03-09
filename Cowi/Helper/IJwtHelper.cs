using Cowi.Models;

namespace Cowi.Helper
{
    public interface IJwtHelper
    {
        string GenerateJwtToken(User user);
        string ValidateJwtToken(string token);
    }
}
