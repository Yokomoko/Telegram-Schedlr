using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Schedlr.POCO;
using LiteDB;
using Telegram.Bot.Args;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Schedlr {

    internal class Program {

        #region Public Fields

        public static TelegramBotClient BotClient = new TelegramBotClient(""); // Your telegram bot id
        public const long ChatId = -0;

        public static string DatabaseName => Properties.Settings.Default.DatabaseName;
        


        #endregion Public Fields

        #region Public Methods

        public static async void TimerCallback(Object o) {
            Console.Clear();
            using (var db = new LiteDatabase(DatabaseName)) {
                var scheduleCollection = db.GetCollection<Schedule>("schedules");
                var schedules = scheduleCollection.FindAll();

                foreach (Schedule schedule in schedules) {
                    if (!schedule.LastRunDateTime.HasValue || schedule.LastRunDateTime.Value.AddHours(schedule.IntervalHours) <= DateTime.Now) {
                        try {
                            await BotClient.SendTextMessageAsync(ChatId, schedule.Message, schedule.ParseMode, true, false);
                        }
                        catch (Exception e) {
                            Console.WriteLine($"Failed to send: {e.Message}");
                        }

                        schedule.LastRunDateTime = DateTime.Now;
                        scheduleCollection.Update(schedule);

                        Console.WriteLine($"Sent Schedule {schedule.Id} at {schedule.LastRunDateTime}");
                    }
                    else {
                        Console.WriteLine($"Schedule {schedule.Id} not ready - Next Send: {schedule.LastRunDateTime.Value.AddHours(schedule.IntervalHours)} - Current Time: {DateTime.Now}");
                    }
                }
            }
        }

        #endregion Public Methods

        #region Private Methods

        public enum BotCommands {
            schedule,
            setinterval,
            setmsgtext,

            preview,
            previewhtml,
            previewmarkdown,

            send,
            sendhtml,
            sendmarkdown,

            getchatid,

            addmessage,
            addmarkdownmessage,
            addhtmlmessage,

            listschedules,
            showscheduletext,
            deleteschedule,
        }

        private static async void DealWithMessage(MessageEventArgs message) {
            try {
                var adminUsers = await BotClient.GetChatAdministratorsAsync(ChatId);
                var isAdmin = adminUsers.Select(d => d.User).Contains(message.Message.From);

                string returnMessage = string.Empty;
                string messageText;


                foreach (var entity in message.Message.Entities) {
                    if (entity.Type == MessageEntityType.BotCommand) {
                        BotCommands command;
                        var splitMsg = message.Message.Text.Split(' ');

                        if (Enum.TryParse(splitMsg[0].Replace(@"/", "").Replace("@Schedlr", "").ToLower(), out command)) {
                            int scheduleId;
                            double interval;
                            switch (command) {
                                case BotCommands.setinterval:
                                    if (!isAdmin) return;

                                    if (!int.TryParse(message.Message.Text.Split(' ')[1], out scheduleId)) {
                                        await BotClient.SendTextMessageAsync(message.Message.Chat.Id, "Invalid Schedule ID", ParseMode.Default, true, false);
                                        return;
                                    }
                                    if (!double.TryParse(message.Message.Text.Split(' ')[2], out interval)) {
                                        await BotClient.SendTextMessageAsync(message.Message.Chat.Id, "Invalid Interval ID", ParseMode.Default, true, false);
                                        return;
                                    }
                                    Schedule.SetInterval(scheduleId, interval, out returnMessage);
                                    break;
                                case BotCommands.setmsgtext:
                                    if (!int.TryParse(message.Message.Text.Split(' ')[1], out scheduleId)) {
                                        await BotClient.SendTextMessageAsync(message.Message.Chat.Id, "Invalid Schedule ID", ParseMode.Default, true, false);
                                        return;
                                    }
                                    var text = messageText = string.Join(" ", message.Message.Text.Split(' ').Skip(2));
                                    Schedule.SetText(scheduleId, text, out returnMessage);
                                    break;
                                case BotCommands.showscheduletext:
                                    if (!int.TryParse(message.Message.Text.Split(' ')[1], out scheduleId)) {
                                        await BotClient.SendTextMessageAsync(message.Message.Chat.Id, "Invalid Schedule ID", ParseMode.Default, true, false);
                                        return;
                                    }
                                    Schedule.GetMessage(scheduleId, out returnMessage);
                                    break;
                                case BotCommands.preview:
                                    if (!isAdmin) return;
                                    await BotClient.SendTextMessageAsync(message.Message.Chat.Id, message.Message.Text.Remove(0, message.Message.Text.IndexOf(' ') + 1), ParseMode.Default, true, false);
                                    break;
                                case BotCommands.previewhtml:
                                    if (!isAdmin) return;
                                    await BotClient.SendTextMessageAsync(message.Message.Chat.Id, message.Message.Text.Remove(0, message.Message.Text.IndexOf(' ') + 1), ParseMode.Html, true, false);
                                    break;
                                case BotCommands.previewmarkdown:
                                    if (!isAdmin) return;
                                    await BotClient.SendTextMessageAsync(message.Message.Chat.Id, message.Message.Text.Remove(0, message.Message.Text.IndexOf(' ') + 1), ParseMode.Markdown, true, false);
                                    break;
                                case BotCommands.send:
                                    if (!isAdmin) return;
                                    await BotClient.SendTextMessageAsync(ChatId, message.Message.Text.Remove(0, message.Message.Text.IndexOf(' ') + 1), ParseMode.Default, true, false);
                                    break;
                                case BotCommands.sendhtml:
                                    if (!isAdmin) return;
                                    await BotClient.SendTextMessageAsync(ChatId, message.Message.Text.Remove(0, message.Message.Text.IndexOf(' ') + 1), ParseMode.Html, true, false);
                                    break;
                                case BotCommands.sendmarkdown:
                                    if (!isAdmin) return;
                                    await BotClient.SendTextMessageAsync(ChatId, message.Message.Text.Remove(0, message.Message.Text.IndexOf(' ') + 1), ParseMode.Markdown, true, false);
                                    break;

                                case BotCommands.getchatid:
                                    if (!isAdmin) return;
                                    await BotClient.SendTextMessageAsync(message.Message.Chat.Id, message.Message.Chat.Id.ToString(), ParseMode.Default, true, false);
                                    break;
                                case BotCommands.listschedules:
                                    if (!isAdmin) return;
                                    await BotClient.SendTextMessageAsync(message.Message.Chat.Id, Schedule.ListSchedules(), ParseMode.Html, true, false);
                                    break;
                                case BotCommands.addmessage:
                                case BotCommands.addhtmlmessage:
                                case BotCommands.addmarkdownmessage:
                                    if (!isAdmin) return;
                                    var newMessage = string.Join(" ", message.Message.Text.Split(' ').Skip(2));
                                    if (!double.TryParse(message.Message.Text.Split(' ')[1], out interval)) {
                                        await BotClient.SendTextMessageAsync(message.Message.Chat.Id, "Invalid Interval Set", ParseMode.Default, true, false);
                                        return;
                                    }
                                    switch (command) {
                                        case BotCommands.addmessage:
                                            Schedule.AddSchedule(interval, newMessage, ParseMode.Default, out returnMessage);
                                            break;
                                        case BotCommands.addmarkdownmessage:
                                            Schedule.AddSchedule(interval, newMessage, ParseMode.Markdown, out returnMessage);
                                            break;
                                        case BotCommands.addhtmlmessage:
                                            Schedule.AddSchedule(interval, newMessage, ParseMode.Html, out returnMessage);
                                            break;
                                        default:
                                            return;
                                    }
                                    break;
                                case BotCommands.deleteschedule:
                                    if (!int.TryParse(message.Message.Text.Split(' ')[1], out scheduleId)) {
                                        await BotClient.SendTextMessageAsync(message.Message.Chat.Id, "Invalid Schedule ID Set", ParseMode.Default, true, false);
                                        return;
                                    }
                                    Schedule.DeleteSchedule(scheduleId, out returnMessage);
                                    break;
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(returnMessage)) {
                    await BotClient.SendTextMessageAsync(message.Message.Chat.Id, returnMessage, ParseMode.Default, true, false);
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }

        }

        private static void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs) {
            if (messageEventArgs.Message.Chat.Type == ChatType.Private) {
                DealWithMessage(messageEventArgs);
            }
        }

        private static void Main() {
            var me = BotClient.GetMeAsync().Result;
            Console.Title = me.Username;
            BotClient.OnMessage += BotOnMessageReceived;

            Timer t = new Timer(TimerCallback, null, 0, 3599999);
            
            BotClient.StartReceiving();
            Console.ReadLine();
        }

        #endregion Private Methods


    }
}