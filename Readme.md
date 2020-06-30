# Error handling samples

Given the following code

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Test();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }

        public static void Test()
        {
            Run("Error", () =>
            {

                throw new Exception("Exception");

            });
        }

        public static int Test2()
        {
            return Run("Error", () =>
            {

                throw new Exception("Exception");

                return 0;
            });
        }

        public static Task Test3()
        {
            return RunAsync("Error", async () =>
            {
                await Task.Delay(100);
                throw new Exception("Exception");
            });
        }

        public static Task<int> Test4()
        {
            return RunAsync("Error", async () =>
            {
                await Task.Delay(100);
                throw new Exception("Exception");
                return 1;
            });
        }
    }


We might want to simplify it.

>I don't suppose there is a way to take this code and 'Inherit' it in each Method I create so I don't have to put this code in every method, just the inheritance notation?

It is not possible simply because you cannot inherit the same method multiple times within the same class. Runtime would not be able to decide which implementation to use - it is done on the type level and it is possible to inherit the same method from different subclasses.

But it is possible to make it more generic.

## Code structure

### Inheritance/Template Method

Considering inheritance we can split the method onto two parts, first which is not supposed to be changed will have the error handling logic, and the second will be an abstract method supposed to be overriden in a subclass.

See the example in [SomeService.cs](ConsoleApp8/SomeService.cs)

Better way would be using 

### Decorator pattern.

Given an interface and a class implementing it(see [AnotherServiceDecorator.cs](ConsoleApp8/AnotherServiceDecorator.cs)), we create another class which wraps the interface with the error handling logic.


### Something else
The third option is when we try to create some kind of a decorator(but not a decorator) but using class instead of interface - to do this we need to have methods virtual, but anyway I wouldn't call it decorator. It is hard to use(to decorator you can pass any variable of your interface type), you need to have the same constructors as the base class have; and you actually extend only the base class, i.e. if there is another implementation of the base class, the error handling logic will not run there.

### Conclusion
From the three options the first one I would use for a different scenario - e.g. you have an algorithm and you have main logic, but some specific operations are different for every implementation. 

Also the first one is worse than the second one because you cannot apply it to an arbitrary class, but you have to put error hahdling logic into the base class.

The third one I would not use for the error handling at all, it is just an experiment.

In order to keeping some logic separately from the business logic I use decorator.

But you still need to write manually all the methods, i.e. you have 10 methods in your interface, you will have to write ten times in your decorator something like 

    public int Test2(double number) => Run("args:" + number, () => decorated.Test2(number));

## Avoiding boilerplate

C# doesn't provide many tools to simplify writing template code.

I'm aware of the following options:
- generating C# code on build
- modifying binaries after build
- generating code in runtime



### Compile-time code generation
I think the first option is the best, but, unfortunately, at the moment C# doesn't have any good mechanism to do this.
Generating code on build using msbuild is easy, but you would need to parse all files apart from C# compiler, which is wasting of resources and, in my opinion, is not trivial(i.e. it is not a one liner in msbuild code task factory, but rather a separate msbuild task written in c sharp, which means it has to be maintained, etc.). It is strange that I cannot just inject the roslyn compilation process so that my custom code will be executed and will get syntax tree as a parameter.

There are plans to introduce this feature, called Source Generators, to C#, which is exactly what I need

https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/


But at the moment it is not production ready.

### Postsharp
The second option is using something like Postsharp.

I don't know how popular it is, but from time to time I see references to it, at least on SO.
But I don't like approach - it modifies binary after build, for me it is just another source of possible errors. If I generate c sharp code I rely on compiler. If there is an error compiler might find it, or at least I can check the generated code myself and see if there's anything wrong here. The same for generating code in runtime - I rely on dotnet framework.
But Postscript is another tool, and it is not free(I didn't find any free version).


### Dynamic Proxy
The third option is generating decorators in runtime.

You could use dotnet TypeBuilder, or, which I think is better, some library, e.g. 

http://www.castleproject.org/projects/dynamicproxy/

>For example, a class could be proxied to add logging or security checking without making the code aware this functionality has been added.

In my project I use either manually written decorators, or, in my home projects - can be also runtime type generator using TypeBuilder, when it is not difficult to implement. Mostly because of studying purposes. You can see it [here](ConsoleApp8/ViewModelTypeCreator.cs)

There is an example of using TypeBuilder in ViewModelTypeCreator.cs, but I would not recommend using it anywhere because it is more difficult to implement and easier to miss something.
But it works fine in my home projects.

### Conclusion

So, in order to maintain error handling logic the best thing would be to separate it from the business logic and keep it in Decorator.

In order to avoid boilerplate(while writing it manually is also fine) Castle Dynamic Proxy is also a good idea.

But when Source Generators are prod ready - I would use them for sure.
