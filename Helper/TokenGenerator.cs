using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;
using Microsoft.IdentityModel.Tokens;

public class JwtTokenGenerator
{

    public string GenerateToken(string email)
    {
        //Generate a 256 bit strong key
        var cryptoServiceProvider = new RNGCryptoServiceProvider();
        var keyBytes = new byte[32]; // 32 bytes = 256 bits
        cryptoServiceProvider.GetBytes(keyBytes);
        var secretKey = Convert.ToBase64String(keyBytes);

        var issuer = WebConfigurationManager.AppSettings["JwtIssuer"];

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
        new Claim(ClaimTypes.Email, email)
    };

        var token = new JwtSecurityToken(
            issuer: issuer,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1), // Token expiration time (adjust as needed)
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
