using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace __Wpf__App
{
    public class MessageCheckerThread : IDisposable
    {
        private Thread _thread;
        private readonly EmailService _emailService;
        private readonly int _userId;
        private readonly TimeSpan _checkInterval;
        private bool _shouldStop;

        public MessageCheckerThread(EmailService emailService, int userId, TimeSpan checkInterval)
        {
            _emailService = emailService;
            _userId = userId;
            _checkInterval = checkInterval;
        }

        public void Start()
        {
            _shouldStop = false;
            _thread = new Thread(Run);
            _thread.Start();
        }

        public void Stop()
        {
            _shouldStop = true;
            _thread.Join();
        }

        private void Run()
        {
            while (!_shouldStop)
            {
                // Get the last check time for the user
                DateTime lastCheckTime = _emailService.GetLastCheckTimeForUser(_userId);

                // Get new messages since the last check time
                List<Email> newMessages = _emailService.GetNewMessagesForUser(_userId, lastCheckTime);

                if (newMessages.Count > 0)
                {
                    // Raise the NewMessageReceived event for each new message
                    foreach (Email newMessage in newMessages)
                    {
                        _emailService.OnNewMessageReceived(newMessage);
                    }

                    // Update the last check time in the database
                    _emailService.UpdateLastCheckTimeForUser(_userId, DateTime.UtcNow);
                }

                Thread.Sleep(_checkInterval);
            }
        }

        public void Dispose()
        {
            Stop();
            _thread = null;
        }
    }
}
