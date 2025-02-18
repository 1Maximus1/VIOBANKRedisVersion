using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using VIOBANK.Domain.Models;

namespace VIOBANK.Infrastructure
{
    public interface IJwtProvider
    {
        string GenerateToken(User user);
        List<Claim> GetClaimsFromToken(string token);
    }
}
