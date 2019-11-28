using ExamReg.Authentication.Common;

namespace ExamReg.Authentication.Controllers.authentication
{
    public class LoginRequestDTO : DataDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}