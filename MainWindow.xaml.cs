using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;

namespace botik1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static TelegramBotClient botClient;
        private const string token = "7825223808:AAH3__CEurIZObVowrKFM-bLL8HIEB9HQyY";
        public MainWindow()
        {
            InitializeComponent();
            botClient = new TelegramBotClient(token);

            var cts = new CancellationTokenSource();
            botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                new ReceiverOptions { AllowedUpdates = { } },
                cancellationToken: cts.Token
            );
        }
        private Random random = new Random();
        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update.Message?.Text != null)
            {
                var chatId = update.Message.Chat.Id;

                try
                {
                    switch (update.Message.Text)
                    {
                        case "/start":
                            await botClient.SendTextMessageAsync(chatId, $"Привет! Ты сегодня прекрасно выглядишь!🤗", cancellationToken: cancellationToken);
                            break;
                        case "/compliment":
                            await botClient.SendTextMessageAsync(chatId, "Твоя улыбка способна осветить даже самый пасмурный день! ☀️", cancellationToken: cancellationToken);
                            break;
                        case "/game":
                            string rnd = Convert.ToString(random.Next(1,4));
                           switch(rnd)
                            {
                                case "1":
                                    await botClient.SendTextMessageAsync(chatId, "У меня очко, я выиграл", cancellationToken: cancellationToken);
                                    break;
                                case "2":
                                    await botClient.SendTextMessageAsync(chatId, "Блин, ничья", cancellationToken: cancellationToken);
                                    break;
                                case "3":
                                    await botClient.SendTextMessageAsync(chatId, "К твоему счастью, ты выиграл", cancellationToken: cancellationToken);
                                    break;
                            }
                            break;
                        
                        default:
                            await botClient.SendTextMessageAsync(chatId, "На данный момент я не умею создавать сны, но я учусь, и однажды смогу выполнить каждое твоё желание! 😊", cancellationToken: cancellationToken);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    await botClient.SendTextMessageAsync(chatId, $"Ошибка: {ex.Message}", cancellationToken: cancellationToken);
                }
            }
        }

        private async Task<string> GetRandomJokeAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync("https://official-joke-api.appspot.com/random_joke");
                    dynamic joke = JsonConvert.DeserializeObject(response);
                    return $"{joke.setup}\n{joke.punchline}";
                }
            }
            catch (Exception ex)
            {
                return $"Ошибка при получении шутки: {ex.Message}";
            }
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Ошибка: {exception.Message}");
            return Task.CompletedTask;
        }


    }
}