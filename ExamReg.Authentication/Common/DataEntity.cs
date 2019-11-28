using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamReg.Authentication.Common
{
    public class DataEntity
    {
        private bool _IsValidated = true;
        public bool IsValidated
        {
            get
            {
                if (Errors != null && Errors.Count > 0) return _IsValidated = false;
                return this._IsValidated;
            }
        }

        public List<string> Errors { get; private set; }

        public DataEntity()
        {
            Errors = new List<string>();
        }

        public void AddError(string className, string Key, Enum Value)
        {
            if (Errors == null) Errors = new List<string>();
            Errors.Add(className + "." + Key + "." + Value.ToString());
        }

        public string GetErrorMessage()
        {
            return Errors.ToString();
        }
    }
}
