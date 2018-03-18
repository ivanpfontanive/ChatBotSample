namespace MemeGenerator.Models
{
    public class Meme
    {
        public Content Direct { get; set; }
        public Content Markdown { get; set; }
    }

    public class Content
    {
        public string Visible { get; set; }
        public string Masked { get; set; }
    }
}