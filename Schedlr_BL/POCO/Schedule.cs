using System;
using System.Linq;
using System.Text;
using LiteDB;
using Telegram.Bot.Types.Enums;

namespace Schedlr_BL.POCO {
    public class Schedule {
        public static string DatabaseName => Properties.Settings.Default.DatabaseName;

        public int Id { get; set; }
        public string Message { get; set; }
        public double IntervalHours { get; set; }
        public DateTime? LastRunDateTime { get; set; }
        public ParseMode ParseMode { get; set; }



        public static void SetInterval(int ScheduleId, double interval, out string message) {
            using (var db = new LiteDatabase(DatabaseName)) {
                var scheduleCollection = db.GetCollection<Schedule>("schedules");
                var schedule = scheduleCollection.Find(d => d.Id == ScheduleId).FirstOrDefault();

                if (schedule != null) {
                    schedule.IntervalHours = interval;
                    scheduleCollection.Update(schedule);
                    message = "Interval Set";
                }
                else {
                    message = $"No schedule found with ID of {ScheduleId}";
                }
            }
        }

        public static void SetText(int ScheduleId, string text, out string message) {
            using (var db = new LiteDatabase(DatabaseName)) {
                var scheduleCollection = db.GetCollection<Schedule>("schedules");
                var schedule = scheduleCollection.Find(d => d.Id == ScheduleId).FirstOrDefault();

                if (schedule != null) {
                    schedule.Message = text;
                    scheduleCollection.Update(schedule);
                    message = "Message Set";
                }
                else {
                    message = $"No schedule found with ID of {ScheduleId}";
                }
            }
        }

        public static void GetMessage(int ScheduleId, out string message, out ParseMode parsemode) {
            using (var db = new LiteDatabase(DatabaseName)) {
                var scheduleCollection = db.GetCollection<Schedule>("schedules");
                var schedule = scheduleCollection.Find(d => d.Id == ScheduleId).FirstOrDefault();

                message = schedule?.Message ?? $"No schedule found with ID of {ScheduleId}";
                parsemode = schedule?.ParseMode ?? ParseMode.Default;
            }
        }

        public static void AddSchedule(double interval, string message, ParseMode type, out string returnMsg) {
            using (var db = new LiteDatabase(DatabaseName)) {
                var scheduleCollection = db.GetCollection<Schedule>("schedules");

                var schedule = new Schedule { Message = message, IntervalHours = interval, ParseMode = type };
                scheduleCollection.Insert(schedule);
            }
            returnMsg = "Schedule Added";
        }

        public static void DeleteSchedule(int scheduleId, out string returnMsg) {

            using (var db = new LiteDatabase(DatabaseName)) {
                var scheduleCollection = db.GetCollection<Schedule>("schedules");
                scheduleCollection.Delete(scheduleId);
                returnMsg = "Message Removed";
            }
        }

        public static string ListSchedules() {
            using (var db = new LiteDatabase(DatabaseName)) {
                var sb = new StringBuilder();
                sb.Append($"The following schedules are set up:{Environment.NewLine}");
                var schedules = db.GetCollection<Schedule>("schedules");
                foreach (var sched in schedules.FindAll()) {
                    sb.AppendLine($"<code>{sched.Id} - {(sched.Message.Length > 20 ? sched.Message.Substring(0, 20) + "..." : sched.Message)} - Interval: {sched.IntervalHours} hours - Last Sent: {sched.LastRunDateTime?.ToString("g") ?? "Never"} - Type: {sched.ParseMode.ToString()}</code>");
                }
                return sb.ToString();
            }
        }
    }
}