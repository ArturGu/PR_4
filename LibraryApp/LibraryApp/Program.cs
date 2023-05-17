using System;
using System.Threading;

namespace LibraryApp
{
    class Library
    {
        public delegate void BookHandler(string message);
        private BookHandler notify;
        private int currentStock;

        public event BookHandler Notify
        {
            add
            {
                notify += value;
                Console.WriteLine($"{value.Method.Name} додано");
            }
            remove
            {
                notify -= value;
                Console.WriteLine($"{value.Method.Name} видалено");
            }
        }

        public int CurrentStock
        {
            get { return currentStock; }
            private set
            {
                currentStock = value;
                notify?.Invoke($"Кiлькiсть книг на складi змiнено i дорiвнює: {currentStock}");
            }
        }

        public void AddBooks(int quantity)
        {
            if (CurrentStock + quantity <= 50)
            {
                CurrentStock += quantity;
                notify?.Invoke($"Додано книг на склад: {quantity}");
            }
            else
            {
                notify?.Invoke($"На складi недостатньо мiсця. Не вдалося додати: {quantity}");
            }
        }

        public void RemoveBooks(int quantity)
        {
            if (CurrentStock - quantity >= 0)
            {
                CurrentStock -= quantity;
                notify?.Invoke($"Вiдвантажено книг зi складу: {quantity}");
            }
            else
            {
                notify?.Invoke($"На складi недостатньо книг для вiдвантаження: {quantity}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Library library = new Library();
            library.Notify += ShowMessage;
            library.AddBooks(10);
            library.AddBooks(20);
            library.AddBooks(20);
            library.AddBooks(20);
            library.RemoveBooks(5);
            library.RemoveBooks(25);

            Console.ReadKey();
        }

        static void ShowMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Thread.Sleep(500);
                Console.WriteLine(message);
            }
        }
    }
}
