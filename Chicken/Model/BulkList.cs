﻿using Newtonsoft.Json;

namespace Chicken.Model
{
    public class BulkList
    {
        [JsonProperty("next_cursor_str")]
        public string NextCursor { get; set; }

        [JsonProperty("previous_cursor_str")]
        public string PreviousCursor { get; set; }
    }
}
