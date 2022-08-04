using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using System.Text.RegularExpressions;

namespace Starter_Csharp
{
    class Program
    {
        // на TimeSpan-ах + проверка формата ввода и формат вывода hh\:mm
        static TelegramBotClient Bot;

        const string COMMAND_LIST =
            @"Бот предназначен для расчёта начала последней трети ночи.
Введите время магриба и фаджра через двоеточие и пробел, например 20:08 2:42
";
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Некоторые действия
            //Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                TimeSpan text;
                string txtAnsw;
                var message = update.Message;
                string pattern = @"\d{2}:\d{2} \d{1,2}:\d{2}$";
                if (message.Text.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat, COMMAND_LIST);
                    return;
                }
                else if (Regex.IsMatch(message.Text, pattern))
                {
                    text = Calculations(message.Text);
                    txtAnsw = "Время начала трети ночи " + text.ToString(@"hh\:mm");
                    if (text == TimeSpan.MinValue)
                        txtAnsw = "Введите время магриба и фаджра в указанном формате через пробел";
                    await botClient.SendTextMessageAsync(message.From.Id, txtAnsw);
                }
                else
                {
                    txtAnsw = "Введите время магриба и фаджра в указанном формате через пробел";
                    await botClient.SendTextMessageAsync(message.From.Id, txtAnsw);
                }
                
            }
        }
        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
            
        }

        private static TimeSpan Calculations(string message)
        {
            TimeSpan polN = new TimeSpan(24, 00, 00);

            try
            {
                var userMagr = TimeSpan.Parse(message.Substring(0, message.IndexOf(' ')));
                var userFajr = TimeSpan.Parse(message.Substring(message.IndexOf(' ')));
                var numb = userFajr - (userFajr + polN - userMagr).Divide(3);

                return numb;
            }
            catch
            {
                return TimeSpan.MinValue;
            }

        }

        static void Main(string[] args)
        {

            Bot = new TelegramBotClient("5461351679:AAF9SqAd42e1EK3tYwDED4gsnJkoMXz9xMk");

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };

            Bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();

        }
    }
}
