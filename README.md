# ActionRateLimiter
Small C# class to enable the limitation of actions performed to a specific rate, using the token bucket algorithm.

See arl-app project for usage example.

Can be used for void-returning actions...
```csharp
// 300 actions per Minute
var limiter = newActionRateLimiter(300, 60);

await limiter.Perform(() => {
    ...
});
```

... and for value-returning action.
```csharp
// 300 actions per Minute
var limiter = newActionRateLimiter(300, 60);

var result = await limiter.Perform<TResult>(() => {
    return ... ;
});
```

---

> **Notice:**
>
> .NET has its own working implementation of the token bucket algorithm in `System.Threading.RateLimiting.TokenBucketRateLimiter`.
> This ist more like a "build it to understand it" project.