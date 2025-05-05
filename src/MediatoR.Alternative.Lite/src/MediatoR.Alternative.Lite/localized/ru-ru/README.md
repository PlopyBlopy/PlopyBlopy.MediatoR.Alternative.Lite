# MediatoR.Alternative.Lite

Упрощённая альтернатива MediatR с базовыми функциями для обработки запросов с поддержкой pipeline behaviors.

## Особенности

- 🚀 Минималистичная реализация медиатора (Sender + Pipeline)
- 🔗 Поддержка middleware-like behaviors (валидация, логирование и т.д.)
- ✅ Интеграция с FluentResults для обработки ошибок
- 💉 Авторегистрация обработчиков через DI (Microsoft.Extensions.DependencyInjection)
- 🛠️ Расширяемая архитектура

## Быстрый старт

### 1. Регистрация сервисов

```csharp
services
    .AddMediatorAlt() // Авторегистрация всех IRequestHandler в текущей сборке
    // Опциональные поведения (порядок регистрации = порядок выполнения):
    .AddMediatorAltLogging()     // Логирование (внешний слой)
    .AddMediatorAltFluentValidation(); // Валидация (внутренний слой)
```

### 2. Создание запроса
```csharp
public record GetUserQuery(int UserId) : IRequest<User>;
```

### 3. Реализация обработчика
```csharp
public class GetUserHandler : IRequestHandler<GetUserQuery, User>
{
    public async Task<Result<User>> Handle(GetUserQuery request, CancellationToken ct)
    {
        // Возвращаем результат через FluentResults
        return Result.Ok(new User(...));
    }
}
```

### 4. Использование
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
            var errors = result.Errors; // Доступ к списку ошибок
            // Обработка ошибок
        }
    }
}
```

## Pipeline Behaviors

### Валидация
- При вызове `AddMediatorAltFluentValidation()` автоматически регистрируются все валидаторы, реализующие `IValidator<TRequest>`

Регистрация валидаторов (FluentValidation):
```csharp
public class GetUserValidator : AbstractValidator<GetUserQuery>
{
    public GetUserValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
    }
}
```

### Кастомное поведение
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

// Регистрация с указанием порядка:
services.AddTransient(
	typeof(IPipelineBehavior<,>), typeof(TimingBehavior<,>)); // Выполнится после других зарегистрированных поведений
```

## Принцип работы

### Порядок выполнения
Поведения выполняются в порядке регистрации:
```
[Поведение 1] -> [Поведение 2] -> ... -> [Обработчик]
```
Пример для `.AddLogging().AddValidation()`:
```
[Logging] -> [Validation] -> [Handler]
```

### Особенности реализации
- Обработчики (`IRequestHandler<,>`) регистрируются как Scoped
- Поведения (`IPipelineBehavior<,>`) выполняются в порядке регистрации
- Все ошибки агрегируются через `FluentResults.Result`

## Лучшие практики

1. **Один запрос - один обработчик**
2. **Используйте `Result<T>` (FluentResults) для возврата данных**
3. **Разделяйте логику:**
   - Валидация → в ValidationBehavior
   - Бизнес-правила → в обработчике
   - Кросс-резанные проблемы → в PipelineBehaviors
4. **Регистрируйте поведения в нужном порядке**
5. **Обрабатывайте ошибки через `result.Errors`**
