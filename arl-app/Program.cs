namespace arl_app;

using arl_lib;

class Program
{
    static async Task Main(string[] args)
    {
        ActionRateLimiter lmt = new(5, 5);
        Console.WriteLine("ActionRateLimiter for action w/o return value: ");
        for (uint i = 1; i <= 10; i++)
        {
            await lmt.Perform(() => { Console.WriteLine(i); });
        }

        lmt = new(5, 5);
        Console.WriteLine("ActionRateLimiter for action w/ return value: ");
        for (uint i = 1; i <= 10; i++)
        {
            var result = await lmt.Perform(() => { return i; });
            Console.WriteLine(result);
        }
    }
}
