using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Artist
    {
        public string artist { set; get; }
        public string title { set; get; }
        public string album { set; get; }
        public DateTimeOffset release_date { set; get; }
        public string label { set; get; }
        public bool underground { set; get; }
    }
}
