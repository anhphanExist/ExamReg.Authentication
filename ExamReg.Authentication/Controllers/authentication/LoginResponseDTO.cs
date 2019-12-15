﻿using ExamReg.Authentication.Common;

namespace ExamReg.Authentication.Controllers.authentication
{
    public class LoginResponseDTO : DataDTO
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public bool isAdmin { get; set; }
    }
}