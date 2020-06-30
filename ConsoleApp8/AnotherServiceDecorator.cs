using System;
using System.Threading.Tasks;
using static ConsoleApp.RuntimeUtility;
namespace ConsoleApp
{
    public interface IAnotherService
    {
        void Test();
        int Test2(double number);
        Task Test3();
        Task<int> Test4();
    }

    public class AnotherServiceDecorator : IAnotherService
    {
        private IAnotherService decorated;

        public AnotherServiceDecorator(IAnotherService decorated)
        {
            this.decorated = decorated;
        }

        public void Test() => Run("args:no args", () => decorated.Test());
        public int Test2(double number) => Run("args:" + number, () => decorated.Test2(number));
        public Task Test3() => RunAsync("args:no args", () => decorated.Test3());
        public Task<int> Test4() => RunAsync("args:no args", () => decorated.Test4());
    }

    public class AnotherService : IAnotherService
    {
        public void Test()
        {
            throw new Exception("Exception");
        }

        public int Test2(double number)
        {
            throw new Exception("Exception");

            return 0;
        }

        public async Task Test3()
        {
            await Task.Delay(100);
            throw new Exception("Exception");
        }

        public async Task<int> Test4()
        {
            await Task.Delay(100);
            throw new Exception("Exception");
            return 1;
        }
    }
}