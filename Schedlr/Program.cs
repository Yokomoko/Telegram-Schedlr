using System;

namespace Schedlr {

    internal class Program {

        #region Private Methods

        private static void Main() {
            var me = Schedlr_BL.Schedlr_Implementation.BotClient.GetMeAsync().Result;
            Console.Title = me.Username;
            Schedlr_BL.Schedlr_Implementation.BotClient.OnMessage += Schedlr_BL.Schedlr_Implementation.BotOnMessageReceived;

            System.Timers.Timer t = new System.Timers.Timer { Interval = 3600000 };
            t.Start();
            t.Elapsed += Schedlr_BL.Schedlr_Implementation.TimerCallback;
            Schedlr_BL.Schedlr_Implementation.BotClient.StartReceiving();
            Console.ReadLine();
        }
        #endregion Private Methods


    }
}