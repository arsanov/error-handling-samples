using System;
using System.Threading.Tasks;
using static ConsoleApp.RuntimeUtility;

namespace ConsoleApp
{
    public class SomeElseService
    {
        public virtual void Test()
        {
            throw new Exception("Exception");
        }

        public virtual int Test2(double number)
        {
            throw new Exception("Exception");
            return 0;
        }

        public async virtual Task Test3()
        {
            await Task.Delay(100);
            throw new Exception("Exception");
        }

        public async virtual Task<int> Test4()
        {
            await Task.Delay(100);
            throw new Exception("Exception");
            return 1;
        }
    }

    public class SomeElseServiceWithErrorHandling : SomeElseService
    {
        public override void Test() => Run("args:no args", () => base.Test());
        public override int Test2(double number) => Run("args:" + number, () => base.Test2(number));
        public override Task Test3() => RunAsync("args:no args", () => base.Test3());
        public override Task<int> Test4() => RunAsync("args:no args", () => base.Test4());
    }
}