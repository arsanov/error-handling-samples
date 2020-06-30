using System;
using System.Threading.Tasks;
using static ConsoleApp.RuntimeUtility;

namespace ConsoleApp
{
    public abstract class SomeServiceBase
    {
        public void Test()
        {
            Run("args:no args", TestInternal);
        }

        public int Test2(double number)
        {
            return Run("args:" + number, Test2Internal);
        }

        public Task Test3()
        {
            return RunAsync("args:no args", Test3Internal);
        }

        public Task<int> Test4()
        {
            return RunAsync("args:no args", Test4Internal);
        }

        protected abstract void TestInternal();

        protected abstract int Test2Internal();

        protected abstract Task Test3Internal();

        protected abstract Task<int> Test4Internal();
    }

    public class SomeService : SomeServiceBase
    {
        protected override void TestInternal()
        {
            throw new Exception("Exception");
        }

        protected override int Test2Internal()
        {
            throw new Exception("Exception");

            return 0;
        }

        protected override async Task Test3Internal()
        {
            await Task.Delay(100);
            throw new Exception("Exception");
        }

        protected override async Task<int> Test4Internal()
        {
            await Task.Delay(100);
            throw new Exception("Exception");
            return 1;
        }
    }


}