using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExamReg.Authentication.Entities;
using ExamReg.Authentication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExamReg.Authentication.Controllers.authentication
{
    [Route("api/ExamReg")]
    public class AuthenticationController : ControllerBase
    {
        private IUserService userService;
        public AuthenticationController(IUserService userService)
        {
            this.userService = userService;
        }

        [Route("login"), HttpPost]
        public async Task<LoginResponseDTO> Login([FromBody] LoginRequestDTO loginDTO)
        {
            User user = new User()
            {
                Username = loginDTO.Username,
                Password = loginDTO.Password
            };

            // Gọi hàm Login trong service
            User res = await this.userService.Login(user);

            // Nếu Login thành công (không có lỗi) thì gán cookies bằng chuỗi Jwt
            if (res.Errors.Count == 0)
            {
                return new LoginResponseDTO()
                {
                    Username = res.Username,
                    Errors = res.Errors,
                    Token = res.Jwt
                };
            }
            else
                return new LoginResponseDTO()
                {
                    Username = res.Username,
                    Errors = res.Errors
                };
            
        }

        [Route("change-password"), HttpPost]
        public async Task<ChangePasswordResponseDTO> ChangePassword([FromBody] ChangePasswordRequestDTO changePasswordDTO)
        {
            User user = new User()
            {
                Username = changePasswordDTO.Username,
                Password = changePasswordDTO.Password
            };
            User result = await this.userService.ChangePassword(user, changePasswordDTO.NewPassword);

            return new ChangePasswordResponseDTO
            {
                Username = result.Username,
                Errors = result.Errors
            };
        }
    }
}