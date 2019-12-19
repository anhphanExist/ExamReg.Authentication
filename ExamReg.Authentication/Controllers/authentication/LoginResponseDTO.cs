using ExamReg.Authentication.Common;
using System;

namespace ExamReg.Authentication.Controllers.authentication
{
    public class LoginResponseDTO : DataDTO
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public DateTime? ExpiredTime { get; set; }
        public bool IsAdmin { get; set; }
    }
}