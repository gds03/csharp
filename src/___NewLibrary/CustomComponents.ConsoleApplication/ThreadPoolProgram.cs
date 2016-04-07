//using CustomComponents.Algorithms.Threading;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace CustomComponents.ConsoleApplication
//{
//    class ThreadPoolProgram
//    {
//        const int ITERATIONS_FOREACH = 1000000;
//        const int NUM_CALLS_FOREACH = 32;
//        static System.Threading.CountdownEvent cde;

//        static void Main(string[] args)
//        {
//            SimpleThreadPool sThreadPool = new SimpleThreadPool();

//        WORKLABEL:
//            cde = new System.Threading.CountdownEvent(4 * NUM_CALLS_FOREACH);

//            for (int i = 0; i < NUM_CALLS_FOREACH; i++)
//            {
//                sThreadPool.QueueWorkItem(Work1);
//                sThreadPool.QueueWorkItem(Work2);
//                sThreadPool.QueueWorkItem(Work3);
//                sThreadPool.QueueWorkItem(Work4);
//            }

//            while (!cde.IsSet)
//            {
//                Console.WriteLine("BUSY THREADS: {0} ", sThreadPool.BusyThreads);
//                Console.WriteLine("Suspended THREADS: {0}", sThreadPool.SuspendedThreads);
//                Thread.Sleep(1000);
//            }


//            // cde.Wait();
//            Console.WriteLine("All work is done");
//            Console.ReadLine();
//            Console.WriteLine("BUSY THREADS: {0} ", sThreadPool.BusyThreads);
//            Console.WriteLine("Suspended THREADS: {0}", sThreadPool.SuspendedThreads);
//            Console.ReadLine();
//            goto WORKLABEL;
//        }


//        static void Work1()
//        {
//            Console.WriteLine("ThreadID: {0}, WORK1 - STARTED", Thread.CurrentThread.ManagedThreadId);
//            List<object> l = new List<object>();

//            for (int i = 0; i < ITERATIONS_FOREACH; i++)
//            {
//                l.Add(i);
//            }
//            cde.Signal();

//            Console.WriteLine("WORK1 - Finished");
//        }

//        static void Work2()
//        {
//            Console.WriteLine("ThreadID: {0}, WORK2 - STARTED", Thread.CurrentThread.ManagedThreadId);
//            LinkedList<object> l = new LinkedList<object>();

//            for (int i = 0; i < ITERATIONS_FOREACH; i++)
//            {
//                l.AddLast(i);
//            }
//            cde.Signal();

//            Console.WriteLine("WORK2 - Finished");
//        }

//        static void Work3()
//        {
//            Console.WriteLine("ThreadID: {0}, WORK3 - STARTED", Thread.CurrentThread.ManagedThreadId);
//            Dictionary<int, object> l = new Dictionary<int, object>();

//            for (int i = 0; i < ITERATIONS_FOREACH; i++)
//            {
//                l.Add(i, new object());
//            }
//            cde.Signal();

//            Console.WriteLine("WORK3 - Finished");
//        }

//        static void Work4()
//        {
//            Console.WriteLine("ThreadID: {0}, WORK4 - STARTED", Thread.CurrentThread.ManagedThreadId);
//            HashSet<int> l = new HashSet<int>();

//            for (int i = 0; i < ITERATIONS_FOREACH; i++)
//            {
//                l.Add(i);
//            }
//            cde.Signal();

//            Console.WriteLine("WORK4 - Finished");
//        }
//    }
//}
