using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceBike.Model.Classes
{
    public class SmartStopwatch
    {
        #region Properties
        public TimeSpan Elapsed => _stopwatch.Elapsed + _timespan;
        public bool IsRunning => _stopwatch.IsRunning;
        #endregion

        #region Private fields
        private readonly Stopwatch _stopwatch;
        private TimeSpan _timespan;
        #endregion

        #region Constructor
        public SmartStopwatch()
        {
            _stopwatch = new Stopwatch();
            _timespan = TimeSpan.Zero;
        }
        #endregion

        #region Public methods
        public void Add(TimeSpan timeSpan)
        {
            _timespan = timeSpan;
        }

        public void Start() { _stopwatch.Start(); }
        public void Stop() { _stopwatch.Stop(); }
        public void Reset() {  _stopwatch.Reset(); }
        public void Restart()
        {
            _stopwatch.Restart();
            _timespan = TimeSpan.Zero;
        }
        #endregion
    }
}
