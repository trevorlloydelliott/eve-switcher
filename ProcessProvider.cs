using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Threading;

namespace EveSwitcher
{
    public class ProcessProvider
    {
        private readonly Timer _timer;
        private Process[] _processes;

        public ProcessProvider()
        {
            _timer = new Timer
            {
                Interval = 1000,
            };
            _timer.Elapsed += Timer_Elapsed;
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public Process[] GetProcesses()
        {
            return _processes;
        }

        public Process GetProcessById(int id)
        {
            return _processes.FirstOrDefault(x => x.Id == id);
        }

        private void Timer_Elapsed(object sender, EventArgs e)
        {
            _processes = Process.GetProcessesByName("exefile");
        }
    }
}
