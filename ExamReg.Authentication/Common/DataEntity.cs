using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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

    public class FilterEntity
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public OrderType OrderType { get; set; }

        public FilterEntity()
        {
            Skip = 0;
            Take = Int32.MaxValue;
            OrderType = OrderType.ASC;
        }
    }

    public enum OrderType
    {
        ASC,
        DESC
    }
}
