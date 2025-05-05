# MediatoR.Alternative.Lite

A simplified alternative to MediatR with core features for request handling and pipeline behavior support.

## Features

- 🚀 Minimalistic mediator implementation (Sender + Pipeline)
- 🔗 Middleware-like behavior support (validation, logging, etc.)
- ✅ FluentResults integration for error handling
- 💉 Auto-registration of handlers via DI (Microsoft.Extensions.DependencyInjection)
- 🛠️ Extensible architecture

## Quick Start

### 1. Register Services

```csharp
services
    .AddMediatorAlt() // Auto-registers all IRequestHandlers in the current assembly
    // Optional behaviors (registration order = execution order):
    .AddMediatorAltLogging()     // Logging (outer layer)
    .AddMediatorAltFluentValidation(); // Validation (inner layer)
```

### 2. Create Request
```csharp
public record GetUserQuery(int UserId) : IRequest<User>;
```

### 3. Implement Handler
```csharp
public class GetUserHandler : IRequestHandler<GetUserQuery, User>
{
    public async Task<Result<User>> Handle(GetUserQuery request, CancellationToken ct)
    {
        // Return result via FluentResults
        return Result.Ok(new User(...));
    }
}
```

### 4. Usage
```csharp
public class MyService
{
    private readonly ISender _sender;

    public MyService(ISender sender) => _sender = sender;

    public async Task Execute()
    {
        var result = await _sender.Send(new GetUserQuery(123));

        if (result.IsSuccess)
        {
            var user = result.Value;
        }
        else
        {
            var errors = result.Errors; // Access error list
            // Handle errors
        }
    }
}
```

## Pipeline Behaviors

### Validation
- Calling `AddMediatorAltFluentValidation()` auto-registers all validators implementing `IValidator<TRequest>`

Register validators (FluentValidation):
```csharp
public class GetUserValidator : AbstractValidator<GetUserQuery>
{
    public GetUserValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
    }
}
```

### Custom Behavior
```csharp
public class TimingBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<Result<TResponse>> Handle(
        TRequest request,
        CancellationToken ct,
        Func<Task<Result<TResponse>>> next)
    {
        var sw = Stopwatch.StartNew();
        var response = await next();
        sw.Stop();
        
        Console.WriteLine($"Execution time: {sw.ElapsedMilliseconds}ms");
        return response;
    }
}

// Registration with order specification:
services.AddTransient(
	typeof(IPipelineBehavior<,>), typeof(TimingBehavior<,>)); // Executes after other registered behaviors
```

## Implementation Details

### Execution Order
Behaviors execute in registration order:
```
[Behavior 1] -> [Behavior 2] -> ... -> [Handler]
```
Example for `.AddLogging().AddValidation()`:
```
[Logging] -> [Validation] -> [Handler]
```

### Key Implementation Notes
- Handlers (`IRequestHandler<,>`) are registered as Scoped
- Behaviors (`IPipelineBehavior<,>`) execute in registration order
- All errors are aggregated via `FluentResults.Result`

## Best Practices

1. **One request - one handler**
2. **Use `Result<T>` (FluentResults) for data returns**
3. **Separate concerns:**
   - Validation → in ValidationBehavior
   - Business rules → in handlers
   - Cross-cutting concerns → in PipelineBehaviors
4. **Register behaviors in required order**
5. **Handle errors via `result.Errors`**