using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Snippet
    {
        public string id { set; get; }
        public string title { set; get; }
        public Shared.Enums.YoutubeSnippetKind kind { set; get; }
    }
}
