using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emotiv2GoogleFit
{
    public class CogniApp
    {
        public class Request
        {
            [Serializable]
            public class BaseRequest
            {
                public int requestId = 0;
                public int session = 0;
                public string type = "";
            }
            [Serializable]
            public class Hello : BaseRequest
            {
                public const string Type = "hello";
                public Hello()
                {
                    type = Type;
                }
                public string message = "";
            }
            [Serializable]
            public class MentalStateData : BaseRequest
            {
                public const string Type = "mental-state-data";
                public MentalStateData()
                {
                    type = Type;
                }
                public decimal? interest;
                public decimal? stress;
                public decimal? relaxation;
                public decimal? engagement;
                public decimal? excitement;
                public decimal? longtermexcitement;
                public decimal? focus;

            }

        }
    }
}
