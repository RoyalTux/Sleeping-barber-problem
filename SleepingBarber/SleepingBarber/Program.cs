using System;
using System.Threading;

namespace SleepingBarber
{
    class Program
    {
        static void Main(string[] args)
        {
            Random Rand = new Random();
            const int MaxCustomers = 10;
            const int NumChairs = 3;
            Semaphore waitingRoom = new Semaphore(NumChairs, NumChairs);
            Semaphore barberChair = new Semaphore(1, 1);
            Semaphore barberSleepChair = new Semaphore(0, 1);
            Semaphore seatBelt = new Semaphore(0, 1);
            bool AllDone = false;
            void Barber()
            {
                while (!AllDone)
                {
                    Console.WriteLine("Парикмахер спит...");
                    barberSleepChair.WaitOne();
                    if (!AllDone)
                    {
                        Console.WriteLine("Парикмахер стрижет...");
                        Thread.Sleep(Rand.Next(1, 3) * 1000);
                        Console.WriteLine("Парикмахер постриг!");
                        seatBelt.Release();
                    }
                    else
                    {
                        Console.WriteLine("Парикмахер спит...");
                    }
                }
                return;
            }
            void Customer(Object number)
            {
                int Number = (int)number;
                Console.WriteLine("Клиент {0} идет в парикмахерскую...", Number);
                Thread.Sleep(Rand.Next(1, 5) * 1000);
                Console.WriteLine("Клиент {0} пришел!", Number);
                waitingRoom.WaitOne();
                Console.WriteLine("Клиент {0} заходит в комнату ожидания...", Number);
                barberChair.WaitOne();
                waitingRoom.Release();
                Console.WriteLine("Клиент {0} будит парикмахера...", Number);
                barberSleepChair.Release();
                seatBelt.WaitOne();
                barberChair.Release();
                Console.WriteLine("Клиент {0} покидает парикмахерскую...", Number);
            }
            Thread BarberThread = new Thread(Barber);
                BarberThread.Start();
                Thread[] Customers = new Thread[MaxCustomers];
                for (int i = 0; i < MaxCustomers; i++)
                {
                    Customers[i] = new Thread(new ParameterizedThreadStart(Customer));
                    Customers[i].Start(i);
                }
                for (int i = 0; i < MaxCustomers; i++)
                {
                    Customers[i].Join();
                }
                AllDone = true;
                barberSleepChair.Release();
                // ждем пока поток парикмахера закончится
                BarberThread.Join();
                Console.WriteLine("Конец работы!");
        }
    }
}
