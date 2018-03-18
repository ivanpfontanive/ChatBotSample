using System;

namespace MemeGenerator.Models
{
    [Serializable]
    public class MemeTemplate
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}