

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace testRestApi.Model
{
    public class GeoDataResponse
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Name { get; set; }
    }

    [DataContract]
    public class RootObject
    {
        [DataMember]
        public string place_id { get; set; }
        [DataMember]
        public string licence { get; set; }
        [DataMember]
        public string osm_type { get; set; }
        [DataMember]
        public string osm_id { get; set; }
        [DataMember]
        public string lat { get; set; }
        [DataMember]
        public string lon { get; set; }
        [DataMember]
        public string display_name { get; set; }

    }
}
