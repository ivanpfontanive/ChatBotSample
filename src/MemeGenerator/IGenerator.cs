﻿using MemeGenerator.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MemeGenerator
{
    public interface IGenerator
    {
        Task<IList<MemeTemplate>> GetMemesTemplateAsync();

        Task<Meme> GetMemeAsync(string templateNome, string texto1, string texto2);
    }
}