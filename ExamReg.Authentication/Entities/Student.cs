using ExamReg.Authentication.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamReg.Authentication.Entities
{
    public class Student : DataEntity
    {
        public Guid Id { get; set; }
        public int StudentNumber { get; set; }
        public string LastName { get; set; }
        public string GivenName { get; set; }
        public DateTime Birthday { get; set; }
        public string Email { get; set; }
    }
}
