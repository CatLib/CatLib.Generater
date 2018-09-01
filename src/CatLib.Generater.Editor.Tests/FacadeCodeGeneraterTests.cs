/*
 * This file is part of the CatLib package.
 *
 * (c) Yu Bin <support@catlib.io>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * Document: http://catlib.io/
 */

using System;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CatLib.Generater.Editor.Tests
{
    [TestClass]
    public class FacadeCodeGeneraterTests
    {
        public interface ITestFacadeCodeGenerater
        {
            void TestWrite();

            string TestRead(int b = 10);

            event Action<Exception> OnClosed;
        }

        [TestMethod]
        public void TestFacadeCodeGenerater()
        {
            var generater = new FacadeCodeGenerater();
            generater.SetGenerateTypes(new Type[]
            {
                typeof(ITestFacadeCodeGenerater)
            });
           
            var generating = generater.Generate(
                new DefaultEnvironment(Path.Combine(Environment.CurrentDirectory, "FacadeCodeGeneraterTests")));

            Exception exception = null;
            generater.OnException += (ex) =>
            {
                exception = ex;
            };
            while (!generating.IsCompleted && exception == null)
            {
                Console.WriteLine(generating.Process);
                Thread.Sleep(1);
            }
            Console.WriteLine(generating.Process);
            if (exception != null)
            {
                throw exception;
            }
        }
    }
}

