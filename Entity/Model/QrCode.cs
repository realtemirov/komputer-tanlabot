    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

namespace bot.Entity.Model
{    public partial class QrCode
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("symbol")]
        public List<Symbol> Symbol { get; set; }
    }

    public partial class Symbol
    {
        [JsonProperty("seq")]
        public long Seq { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("error")]
        public object Error { get; set; }
    }
}
