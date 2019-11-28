using ExamReg.Authentication.Common;
using ExamReg.Authentication.Entities;
using ExamReg.Authentication.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ExamReg.Authentication.Services
{
    public interface IUserService : IServiceScoped
    {
        Task<User> Login(User user);
        Task<User> ChangePassword(User user, string newPassword);
        Task<User> Create(User user);
    }
    public class UserService : IUserService
    {
        public enum ErrorCode
        {
            GenerateJWTError
        }

        private AppSettings appSettings;
        private IUOW UOW;
        private IUserValidator UserValidator;
        public UserService(IUOW UOW, IOptions<AppSettings> options)
        {
            this.UOW = UOW;
            this.appSettings = options.Value;
        }
        public Task<User> ChangePassword(User user, string newPassword)
        {
            throw new NotImplementedException();
        }

        public Task<User> Create(User user)
        {
            throw new NotImplementedException();
        }

        public async Task<User> Login(User user)
        {
            // try authenticate user
            if (!await UserValidator.Login(user))
                return user;

            UserFilter userFilter = new UserFilter
            {
                Username = user.Username,
                Password = user.Password
            };

            user = await UOW.UserRepository.Get(userFilter);

            // authentication successful so generate jwt token
            user = await this.GenerateJWT(user, appSettings.JWTSecret, appSettings.JWTLifeTime);
            return user;
        }

        private async Task<User> GenerateJWT(User user, string jWTSecret, int jWTLifeTime)
        {
            if (string.IsNullOrEmpty(jWTSecret) || jWTLifeTime <= 0)
                user.AddError(nameof(UserService), nameof(GenerateJWT), ErrorCode.GenerateJWTError);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jWTSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim("IsAdmin", user.IsAdmin.ToString())
                }),
                Expires = DateTime.UtcNow.AddSeconds(jWTLifeTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            user.Jwt = tokenHandler.WriteToken(securityToken);
            user.ExpiredTime = tokenDescriptor.Expires;
            return user;
        }
    }
}
