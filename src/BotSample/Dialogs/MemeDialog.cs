using MemeGenerator;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
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

            int length = (activity.Text ?? string.Empty).Length;

            // return our reply to the user
            await context.PostAsync(length.ToString());

            context.Wait(MessageReceivedAsync);
        }
    }
}