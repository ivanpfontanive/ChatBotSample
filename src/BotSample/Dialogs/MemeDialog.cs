using AdaptiveCards;
using MemeGenerator;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;
using Resources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BotSample.Dialogs
{
    [Serializable]
    public class MemeDialog : IDialog<object>
    {
        public IGenerator Generator { get; }

        public MemeDialog(IGenerator generator)
        {
            Generator = generator;
        }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (activity.Value != null)
            {
                await ProcessarSubmitAsync(context, activity);
            }
            else
            {
                await IniciarMemesAsync(context, activity);
            }

            context.Wait(MessageReceivedAsync);
        }

        private async Task IniciarMemesAsync(IDialogContext context, Activity activity)
        {
            await context.PostAsync(ResMensagens.msgMemeBemVindo);
            var message = activity.CreateReply();

            var adaptiveCard = new AdaptiveCard();

            adaptiveCard.Body.Add(new AdaptiveTextBlock() { Text = ResMensagens.msgPreenchaFormularioMeme });

            var choices = new AdaptiveChoiceSetInput();
            choices.Id = "template";
            choices.Style = AdaptiveChoiceInputStyle.Compact;

            foreach (var item in await Generator.GetMemesTemplateAsync())
            {
                choices.Choices.Add(new AdaptiveChoice() { Title = item.Name, Value = item.Name });
            }
            adaptiveCard.Body.Add(choices);
            adaptiveCard.Body.Add(new AdaptiveTextInput() { Id = "texto_1", MaxLength = 40, Placeholder = ResMensagens.lblTextoTopoMeme });
            adaptiveCard.Body.Add(new AdaptiveTextInput() { Id = "texto_2", MaxLength = 40, Placeholder = ResMensagens.lblTextoRodapeMeme });

            adaptiveCard.Actions.Add(new AdaptiveSubmitAction() { Title = "Ok", Id = "Submeter" });
            message.Attachments.Add(new Attachment() { ContentType = AdaptiveCard.ContentType, Content = adaptiveCard });

            await context.PostAsync(message);
        }

        private async Task ProcessarSubmitAsync(IDialogContext context, Activity activity)
        {
            var message = activity.CreateReply();

            var post = JObject.Parse(activity.Value.ToString());

            var template = post["template"].ToString();
            var texto_1 = post["texto_1"].ToString();
            var texto_2 = post["texto_2"].ToString();

            if (string.IsNullOrWhiteSpace(template)
                || string.IsNullOrWhiteSpace(texto_1)
                || string.IsNullOrWhiteSpace(texto_2))
            {
                await context.PostAsync(ResMensagens.msgTemplateTextoObrigatorio);
                return;
            }

            await context.PostAsync("");

            var meme = await Generator.GetMemeAsync(template, texto_1, texto_2);

            await context.PostAsync(ResMensagens.msgAguardeGerarndoMeme);

            var card = new HeroCard();
            card.Title = template;
            var cardMeme = new CardImage() { Url = meme.Direct.Masked, Alt = meme.Direct.Masked };
            card.Images = new List<CardImage>() { cardMeme };
            card.Text = meme.Direct.Masked;
            message.Attachments.Add(card.ToAttachment());

            await context.PostAsync(message);
        }
    }
}