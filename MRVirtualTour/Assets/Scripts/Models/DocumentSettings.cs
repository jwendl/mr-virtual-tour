using System;

namespace VirtualTourApi.Models
{
    public class DocumentSettings
    {
        public Uri DocumentEndpoint { get; set; }

        public string DocumentKey { get; set; }

        public string DatabaseId { get; set; }
    }
}
