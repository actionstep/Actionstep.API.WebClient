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


    //public class ResthookResponseDto
    //{
    //    [JsonProperty("data")] public Data ResthookData { get; set; } = new Data();

    //    public class Data
    //    {
    //        [JsonProperty("type")] public string ResourceType { get; set; }
    //        [JsonProperty("id")] public string ResourceId { get; set; }
    //    }
    //}
}

//{
//    "jsonapi": {
//        "version": "1.0"
//    },
//    "data": {
//        "type": "filenotes",
//        "id": "259",
//        "attributes": {
//            "enteredTimestamp": "2020-06-12T13:23:34+12:00",
//            "text": "resthook test 2",
//            "enteredBy": "Randall, Steve",
//            "source": "User",
//            "noteTimestamp": "2020-06-12T13:23:34+12:00"
//        },
//        "relationships": {
//            "action": {
//                "data": {
//                    "type": "actions",
//                    "id": "5"
//                }
//            },
//            "document": null,
//            "participant": {
//                "data": {
//                    "type": "participants",
//                    "id": "10"
//                }
//            }
//        }
//    },
//    "meta": {
//        "paging": {
//            "filenotes": {
//                "recordCount": 1,
//                "pageCount": 1
//            }
//        },
//        "debug": {
//            "requestTime": "0.024633",
//            "mem": "11.88mb",
//            "server": "app-bc40b47477b7.ap-southeast-2.actionstepstaging.com",
//            "cb": "T1000-6102-1",
//            "time": "2020-06-12 13:23:35 +1200 (Pacific/Auckland)",
//            "appload": "0.15, 0.14, 0.10",
//            "app": "0.000727",
//            "db": "0.023906",
//            "dbc": "0.004737",
//            "qc": "44",
//            "uqc": "34",
//            "fc": "712",
//            "rl": null,
//            "resthook": {
//                "id": "46",
//                "target": "http://172a6dfbabc2.au.ngrok.io/api/resthook",
//                "trigger_count": 1,
//                "event_name": "FileNoteCreated",
//                "resource_type": "filenotes",
//                "resource_id": "259"
//            }
//        }
//    }
//}
