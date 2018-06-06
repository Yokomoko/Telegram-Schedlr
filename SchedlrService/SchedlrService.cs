using System;
using System.ServiceProcess;
using System.Threading;

namespace SchedlrService {
    public partial class SchedlrService : ServiceBase {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main() {
            var ServicesToRun = new ServiceBase[] { new SchedlrService() };
            Run(ServicesToRun);
        }


        public SchedlrService() {
            InitializeComponent();
        }

        protected override void OnStart(string[] args) {
            Thread thread = new Thread(StartService);
            thread.Start();
        }

        protected override void OnStop() {
        }

        private void StartService() {
            var me = Schedlr_BL.Schedlr_Implementation.BotClient.GetMeAsync().Result;
            Console.Title = me.Username;
            Console.SetBufferSize(5, 5);
            Schedlr_BL.Schedlr_Implementation.BotClient.OnMessage += Schedlr_BL.Schedlr_Implementation.BotOnMessageReceived;

            System.Timers.Timer t = new System.Timers.Timer { Interval = 3600000 };
            t.Start();
            t.Elapsed += Schedlr_BL.Schedlr_Implementation.TimerCallback;
            Schedlr_BL.Schedlr_Implementation.BotClient.StartReceiving();
            Console.ReadLine();
        }


    }
}
