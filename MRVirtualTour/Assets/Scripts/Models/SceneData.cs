using Newtonsoft.Json;
using System.Collections.Generic;

namespace VirtualTourApi.Models
{
    public class SceneData
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string StorageContainerName { get; set; }

        public string StorageVideoName { get; set; }

        public IEnumerable<UnityPrimitive> UnityPrimatives { get; set; }
    }
}
