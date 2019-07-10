// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="GSD Logic">
//   Copyright © 2019 GSD Logic. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FactoryPerformance
{
    using System;
    using System.Diagnostics;

    internal class Program
    {
        private static void Main(string[] args)
        {
            const int Iterations = 100000000;

            Test1(Iterations); // 3.148, 3.339, 3.379
            Test2(Iterations); // 16.982, 16.607, 16.405
            Test3(Iterations); // 3.352, 3.057, 3.148

            // TODO: Use TypeBuidler to create a factory.
        }

        private static void Test1(int iterations)
        {
            var factory = new WidgetFactory();
            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
            {
                factory.CreateWidget(i.ToString());
            }

            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0);
        }

        private static void Test2(int iterations)
        {
            var factory = typeof(Widget).GetConstructor(new[] { typeof(string) });
            var parameters = new object[1];
            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
            {
                parameters[0] = i.ToString();
                factory.Invoke(parameters);
            }

            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0);
        }

        private static void Test3(int iterations)
        {
            var factory = new Func<string, Widget>(s => new Widget(s));
            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
            {
                factory.Invoke(i.ToString());
            }

            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0);
        }
    }
}