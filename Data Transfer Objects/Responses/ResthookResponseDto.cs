using System;

namespace Actionstep.API.WebClient.Data_Transfer_Objects.Responses
{
    public class ResthookResponseDto
    {
        public Data data { get; set; } = new Data();

        public class Data
        {
            public string type { get; set; }
            public string id { get; set; }
            public AttributeData attributes { get; set; }


            public class AttributeData
            {
                public DateTime enteredTimestamp { get; set; }
                public string text { get; set; }
                public string enteredBy { get; set; }
            }
        }
    }
}