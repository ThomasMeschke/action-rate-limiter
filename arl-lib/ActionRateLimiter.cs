namespace arl_lib;

public class ActionRateLimiter
{
    private readonly Lock sync = new();

    protected uint maxTokenCount;

    protected DateTime lastAction = DateTime.MinValue;
    protected double currentTokens;
    protected double tokenRate;

    public ActionRateLimiter(uint maxTokenCount, uint amountOfSecondsToRefillTheBucket)
    {
        currentTokens = this.maxTokenCount = maxTokenCount;
        tokenRate = (double)maxTokenCount / amountOfSecondsToRefillTheBucket;
    }

    public async Task Perform(Action action)
    {
        await Wait();

        action.Invoke();
    }

    public async Task<TResult> Perform<TResult>(Func<TResult> action)
    {
        await Wait();

        return action.Invoke();
    }

    protected async Task Wait()
    {
        TimeSpan waitTime;

        lock(sync)
        {
            var now = DateTime.UtcNow;
            var secondsSinceLastAction = (now - lastAction).TotalSeconds;

            // refill Tokens that were regenerated since last Action
            currentTokens += secondsSinceLastAction * tokenRate;
            currentTokens = Math.Min(currentTokens, maxTokenCount);

            lastAction = now;

            // use Token for planned action,
            // currentTokens can turn negative here
            currentTokens--;

            // if tokens are reserved already (negative token count),
            // wait as many seconds as needed to have another token,
            // otherwise just go
            waitTime = currentTokens < 0
                ? TimeSpan.FromSeconds(-currentTokens / tokenRate)
                : TimeSpan.Zero;
        }

        if (waitTime > TimeSpan.Zero)
        {
            await Task.Delay(waitTime);
        }
    }
}
