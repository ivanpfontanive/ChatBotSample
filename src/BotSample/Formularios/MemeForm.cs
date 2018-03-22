﻿using MemeGenerator;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using Microsoft.Bot.Connector;
using Resources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BotSample.Formularios
{
    [Serializable]
    public class MemeForm
    {
        public string Template { get; set; }

        public string TextUp { get; set; }

        public string TextDown { get; set; }

        public static IGenerator generator;

        static MemeForm()
        {
            generator = new Generator();
        }

        public static IForm<MemeForm> BuildForm()
        {
            var templates = generator.GetMemesTemplateAsync().Result;

            var form = new FormBuilder<MemeForm>();
            form.Message(ResMensagens.msgMemeBemVindo);
            form.Field(new FieldReflector<MemeForm>(nameof(Template))
                            .SetType(null)
                            .SetDefine((state, field) =>
                            {
                                foreach (var template in templates)
                                    field
                                        .AddDescription(template.Name, template.Name)
                                        .AddTerms(template.Name, template.Name);

                                return Task.FromResult(true);
                            }));
            form.AddRemainingFields();
            form.Configuration.DefaultPrompt.ChoiceStyle = ChoiceStyleOptions.PerLine;

            form.OnCompletion(async (context, memeForm) =>
            {
                await GerarMemeAsync(context, memeForm);
            });

            return form.Build();
        }

        private static async Task GerarMemeAsync(IDialogContext context, MemeForm memeForm)
        {
            var meme = await generator.GetMemeAsync(memeForm.Template, memeForm.TextUp, memeForm.TextDown);

            await context.PostAsync(ResMensagens.msgAguardeGerarndoMeme);

            var card = new HeroCard();
            card.Title = memeForm.Template;
            var cardMeme = new CardImage() { Url = meme.Direct.Masked, Alt = meme.Direct.Masked };
            card.Images = new List<CardImage>() { cardMeme };
            card.Text = meme.Direct.Masked;

            var message = context.Activity as Activity;
            var ac = message.CreateReply();
            ac.Attachments.Add(card.ToAttachment());

            await context.PostAsync(ac);
        }
    }
}