// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="GSD Logic">
//   Copyright © 2019 GSD Logic. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FactoryPerformance
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Reflection.Emit;

    internal class Program
    {
        private static void Main(string[] args)
        {
            const int Iterations = 100000000;

            Test1(Iterations); // 3.273, 3.268, 3.141 (WidgetFactory)
            Test2(Iterations); // 3.149, 3.093, 3.094 (Delegate)
            Test3(Iterations); // 16.711, 16.574, 16.376 (Reflection)
            Test4(Iterations); // 3.152, 3.127, 3.119 (Emitted IWidgetFactory)
            Test5(Iterations); // 4.622, 4.662, 4.675 (Emitted Delegate)
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
            var factory = new Func<string, Widget>(s => new Widget(s));
            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
            {
                factory.Invoke(i.ToString());
            }

            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0);
        }

        private static void Test3(int iterations)
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

        private static void Test4(int iterations)
        {
            var factoryType = typeof(IWidgetFactory);
            var factoryParameters = new[] { typeof(string) };
            var factoryMethod = factoryType.GetMethod("CreateWidget", factoryParameters);

            var assemblyName = new AssemblyName("WidgetFactories");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

            var typeBuilder = moduleBuilder.DefineType(
                "WidgetFactory",
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout,
                typeof(object),
                new[] { factoryType }
            );

            var methodBuilder = typeBuilder.DefineMethod(
                factoryMethod.Name,
                MethodAttributes.Public |
                MethodAttributes.Final |
                MethodAttributes.HideBySig |
                MethodAttributes.Virtual |
                MethodAttributes.NewSlot,
                factoryMethod.ReturnType,
                factoryParameters
            );

            var il = methodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Newobj, typeof(Widget).GetConstructor(factoryParameters));
            il.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(methodBuilder, factoryMethod);

            var type = typeBuilder.CreateType();

            if (!(Activator.CreateInstance(type) is IWidgetFactory factory))
            {
                throw new InvalidOperationException();
            }

            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
            {
                factory.CreateWidget(i.ToString());
            }

            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0);
        }

        private static void Test5(int iterations)
        {
            var method = new DynamicMethod("CreateWidget", typeof(Widget), new[] { typeof(string) });
            var il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Newobj, typeof(Widget).GetConstructor(new[] { typeof(string) }));
            il.Emit(OpCodes.Ret);

            var factory = (Func<string, Widget>)method.CreateDelegate(typeof(Func<string, Widget>));

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