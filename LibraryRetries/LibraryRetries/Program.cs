using System;
using System.Collections.Generic;
using System.Threading;

namespace LibraryRetries
{
    class Library
    {
        public delegate void BookHandler(string message);
        private Dictionary<int, List<BookHandler>> subscribers;
        private int currentStock;

        public Library()
        {
            subscribers = new Dictionary<int, List<BookHandler>>();
        }

        public int CurrentStock
        {
            get { return currentStock; }
            private set
            {
                currentStock = value;
                NotifySubscribersWithRetry($"Кiлькiсть книг на складi змiнено i дорiвнює: {currentStock}", 3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));
            }
        }

        public void AddBooks(int quantity)
        {
            if (CurrentStock + quantity <= 50)
            {
                CurrentStock += quantity;
                NotifySubscribersWithRetry($"Додано книг на склад: {quantity}", 3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));
            }
            else
            {
                NotifySubscribersWithRetry($"На складi недостатньо мiсця. Не вдалося додати: {quantity}", 3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));
            }
        }

        public void RemoveBooks(int quantity)
        {
            if (CurrentStock - quantity >= 0)
            {
                CurrentStock -= quantity;
                NotifySubscribersWithRetry($"Вiдвантажено книг зi складу: {quantity}", 3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));
            }
            else
            {
                NotifySubscribersWithRetry($"На складi недостатньо книг для вiдвантаження: {quantity}", 3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));
            }
        }

        public void Subscribe(BookHandler handler, int priority)
        {
            if (!subscribers.ContainsKey(priority))
            {
                subscribers[priority] = new List<BookHandler>();
            }

            subscribers[priority].Add(handler);
        }

        public void Unsubscribe(BookHandler handler, int priority)
        {
            if (subscribers.ContainsKey(priority))
            {
                subscribers[priority].Remove(handler);
            }
        }

        private void NotifySubscribers(string message)
        {
            foreach (var priority in subscribers.Keys)
            {
                foreach (var subscriber in subscribers[priority])
                {
                    subscriber.Invoke(message);
                    Thread.Sleep(500);
                }
            }
        }

        private void NotifySubscribersWithRetry(string message, int maxRetries, TimeSpan initialDelay, TimeSpan maxDelay)
        {
            foreach (var priority in subscribers.Keys)
            {
                foreach (var subscriber in subscribers[priority])
                {
                    RetryPolicy.Execute(() => subscriber.Invoke(message), maxRetries, initialDelay, maxDelay);
                    Thread.Sleep(500);
                }
            }
        }
    }

    static class RetryPolicy
    {
        public static void Execute(Action action, int maxRetries, TimeSpan initialDelay, TimeSpan maxDelay)
        {
            var random = new Random();
            var retries = 0;
            var delay = initialDelay;

            while (true)
            {
                try
                {
                    action.Invoke();
                    break;
                }
                catch
                {
                    if (retries >= maxRetries)
                        throw;

                    var randomDelay = random.Next((int)delay.TotalMilliseconds);
                    Thread.Sleep(randomDelay);

                    retries++;
                    delay = TimeSpan.FromTicks(Math.Min(delay.Ticks * 2, maxDelay.Ticks));
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Library library = new Library();
            library.Subscribe(ShowMessageHighPriority, 1);
            library.Subscribe(ShowMessageLowPriority, 2);
            library.Subscribe(ShowMessageMediumPriority, 1);
            library.Subscribe(ShowMessageHighPriority, 1);

            library.AddBooks(10);
            library.AddBooks(20);
            library.AddBooks(20);
            library.AddBooks(20);
            library.RemoveBooks(5);
            library.RemoveBooks(25);

            Console.ReadKey();
        }

        static void ShowMessageHighPriority(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Console.WriteLine($"[Високий прiоритет]: {message}");
            }
        }

        static void ShowMessageMediumPriority(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Console.WriteLine($"[Середнiй прiоритет]: {message}");
            }
        }

        static void ShowMessageLowPriority(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Console.WriteLine($"[Низький прiоритет]: {message}");
            }
        }
    }
}
