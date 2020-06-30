using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
namespace ConsoleApp
{
    public class RuntimeUtility
    {
        public static void Run(string errorMessage, Action action, [CallerMemberName] string name = null)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                throw new Exception(errorMessage + " from method " + name, e);
            }
        }

        public static T Run<T>(string errorMessage, Func<T> action, [CallerMemberName] string name = null)
        {
            try
            {
                return action();
            }
            catch (Exception e)
            {
                throw new Exception(errorMessage + " from method " + name, e);
            }
        }

        public static async Task RunAsync(string errorMessage, Func<Task> action, [CallerMemberName] string name = null)
        {
            try
            {
                await action();
            }
            catch (Exception e)
            {
                throw new Exception(errorMessage + " from method " + name, e);
            }
        }

        public static async Task<T> RunAsync<T>(string errorMessage, Func<Task<T>> action, [CallerMemberName] string name = null)
        {
            try
            {
                return await action();
            }
            catch (Exception e)
            {
                throw new Exception(errorMessage + " from method " + name, e);
            }
        }
    }
}