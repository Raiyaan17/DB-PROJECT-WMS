using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace api.BusinessLogic;

public class AuthService
{
    private readonly WheresMyScheduleDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(WheresMyScheduleDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<string?> LoginStudentAsync(string studentId, string password)
    {
        // 1. Verify fixed password
        if (password != "Student123!@#")
        {
            return null;
        }

        // 2. Check if student exists
        // PK is Composite (GraduationYear, StudentId), so we can't use FindAsync with just StudentId.
        var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
        if (student == null)
        {
            return null;
        }

        // 3. Generate Token
        return GenerateJwtToken(student.StudentId, "Student");
    }

    public async Task<string?> LoginAdminAsync(string username, string password)
    {
        // 1. Verify fixed password
        if (password != "Admin123!@#")
        {
            return null;
        }

        // 2. Check if admin exists
        var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Username == username);
        if (admin == null)
        {
            return null;
        }

        // 3. Generate Token
        return GenerateJwtToken(admin.AdminId, "Admin");
    }

    private string GenerateJwtToken(string userId, string role)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role)
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
