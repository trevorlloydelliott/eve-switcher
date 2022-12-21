using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace EveSwitcher
{
    public class ProcessProvider
    {
        private readonly DispatcherTimer _timer;
        private Process[] _processes;

        public ProcessProvider()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _timer.Tick += Timer_Tick;
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public Process[] GetProcessesByName(string name)
        {
            return _processes.Where(x => x.ProcessName == name).ToArray();            
        }

        public Process GetProcessById(int id)
        {
            return _processes.FirstOrDefault(x => x.Id == id);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _processes = Process.GetProcesses();
        }
    }
}
