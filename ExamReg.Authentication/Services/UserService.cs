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
            GenerateJwtError
        }

        private AppSettings appSettings;
        private IUOW UOW;
        private IUserValidator UserValidator;
        public UserService(IUOW UOW, IUserValidator UserValidator, IOptions<AppSettings> Options)
        {
            this.UOW = UOW;
            this.appSettings = Options.Value;
            this.UserValidator = UserValidator;
        }
        public async Task<User> ChangePassword(User user, string newPassword)
        {
            // Xác thực dữ liệu thay đổi password
            if (!await UserValidator.Update(user, newPassword))
                return user;

            using (UOW.Begin())
            {
                try
                {
                    UserFilter filter = new UserFilter
                    {
                        Username = user.Username,
                        Password = user.Password
                    };
                    user = await UOW.UserRepository.Get(filter);
                    user.Password = newPassword;

                    await UOW.UserRepository.Update(user);
                    await UOW.Commit();
                    return await UOW.UserRepository.Get(new UserFilter
                    {
                        Username = user.Username,
                        Password = newPassword
                    });
                }
                catch (Exception e)
                {
                    await UOW.Rollback();
                    user.AddError(nameof(UserService), nameof(ChangePassword), Common.ErrorCode.SystemError);
                    return user;
                }
            }
        }

        public Task<User> Create(User user)
        {
            throw new NotImplementedException();
        }

        public async Task<User> Login(User user)
        {
            // Xác thực username và password
            if (!await UserValidator.Login(user))
                return user;

            // Xác thực người dùng thành công, lấy thông tin người dùng ra và tạo jwt token
            UserFilter userFilter = new UserFilter
            {
                Username = user.Username,
                Password = user.Password
            };
            user = await UOW.UserRepository.Get(userFilter);
            user = await this.GenerateJWT(user, appSettings.JWTSecret, appSettings.JWTLifeTime);

            // Trả về thông tin người dùng kèm token
            return user;
        }

        private async Task<User> GenerateJWT(User user, string jWTSecret, int jWTLifeTime)
        {
            if (string.IsNullOrEmpty(jWTSecret) || jWTLifeTime <= 0)
                user.AddError(nameof(UserService), nameof(GenerateJWT), ErrorCode.GenerateJwtError);
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
