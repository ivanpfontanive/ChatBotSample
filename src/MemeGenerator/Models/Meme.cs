using System;

namespace MemeGenerator.Models
{
    [Serializable]
    public class Meme
    {
        public Content Direct { get; set; }
        public Content Markdown { get; set; }
    }

    [Serializable]
    public class Content
    {
        public string Visible { get; set; }
        public string Masked { get; set; }
    }
}