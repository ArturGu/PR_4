using System;
using System.Collections.Generic;
using System.Threading;

namespace LibrarySubscriptionPublications
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
                NotifySubscribers($"Кiлькiсть книг на складi змiнено i дорiвнює: {currentStock}");
            }
        }

        public void AddBooks(int quantity)
        {
            if (CurrentStock + quantity <= 50)
            {
                CurrentStock += quantity;
                NotifySubscribers($"Додано книг на склад: {quantity}");
            }
            else
            {
                NotifySubscribers($"На складi недостатньо мiсця. Не вдалося додати: {quantity}");
            }
        }

        public void RemoveBooks(int quantity)
        {
            if (CurrentStock - quantity >= 0)
            {
                CurrentStock -= quantity;
                NotifySubscribers($"Вiдвантажено книг зi складу: {quantity}");
            }
            else
            {
                NotifySubscribers($"На складi недостатньо книг для вiдвантаження: {quantity}");
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
