# Шаблоны проектирования GoF

## Порождающие шаблоны

### Builder
```csharp
NpgsqlConnectionStringBuilder sb = new NpgsqlConnectionStringBuilder();
sb.Host = Environment.GetEnvironmentVariable("POSTGRES_HOST");
sb.Port = int.Parse(Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432");
sb.Username = Environment.GetEnvironmentVariable("POSTGRES_USER");
sb.Password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
sb.Database = Environment.GetEnvironmentVariable("POSTGRES_DATABASE"); 

optionsBuilder.UseNpgsql(sb.ToString());
```

![builder](https://github.com/user-attachments/assets/7fed231c-9f53-49da-a61b-9e5ee94f0be4)

Один из примеров использования паттерна билдера в коде - создание строки подключения к БД. Результирующая строка подключения зависит от конкретной реализации, но параметры одинаковые для ряда БД.

### Singleton
```csharp
class AuthServerClient
{
	private readonly HttpClient _httpClient;

	public static readonly AuthServerClient Instance;

	private AuthServerClient()
	{
		_httpClient = new HttpClient();
	}

	static AuthServerClient()
	{
		Instance = new AuthServerClient();
	}

	public async Task<bool> CheckAsync(string route)
	{
		return (await _httpClient.GetAsync(route)).IsSuccessStatusCode;
	}
}
```

![Singleton](https://github.com/user-attachments/assets/19afbda3-c1f9-4b2f-a87a-1c6b3f610a44)


Пример паттерна Singleton.

## Структурные шаблоны

## Proxy

middleware выполняет роль прокси для эндпоинта, управляя доступом к нему

```csharp
public class AuthMiddleware
{
	public delegate string GetAuthRouteType(string username, string permission);

	protected readonly GetAuthRouteType _getAuthRoute;
	protected readonly RequestDelegate _next;

	public AuthMiddleware(RequestDelegate next, GetAuthRouteType getAuthRoute)
	{
		_next = next;
		_getAuthRoute = getAuthRoute;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		var end = context.GetEndpoint();
		var attr = (AuthPermissionAttribute?)end?.Metadata.FirstOrDefault(x => x is AuthPermissionAttribute);
		Console.WriteLine("Running auth middleware");
		Console.WriteLine(attr);
		if (attr != null)
		{
			string? username = null;
			if (context.Request.Headers.TryGetValue("authorization", out StringValues authHeaders))
			{						
				foreach (var header in authHeaders)
				{
					if (header?.StartsWith("User") == false)
						continue;
					username = header?[(header.IndexOf(" ") + 1)..];
					break;
				}
			}
			if (username != null)
			{
				Console.WriteLine(username);
				Console.WriteLine(attr.Permission);
				string route = _getAuthRoute(username, attr.Permission);
				if (await AuthServerClient.Instance.CheckAsync(route))
				{
					await _next(context);
					return;
				}
			}
			context.Response.StatusCode = 401;
			return;
		}
		else
			await _next(context);
	}
}
```

![Decorator_Proxy_CoR](https://github.com/user-attachments/assets/513bfe88-30f8-4165-8231-ae6dd73487a2)

Middleware является Proxy для остальных Middleware и эндпоинта, регулируя доступ к остальным Middleware и эндпоинту.
В примере кода выше (и ниже) используется by-convention интерфейс Middleware, т.к. он позволяет передать в конструктор дополнительные аргументы при добавлениии в приложение.
Также существует явный интерфейс IMiddleware.

### Decorator

```csharp
public class AuthMiddleware
{
	public delegate string GetAuthRouteType(string username, string permission);

	protected readonly GetAuthRouteType _getAuthRoute;
	protected readonly RequestDelegate _next;

	public AuthMiddleware(RequestDelegate next, GetAuthRouteType getAuthRoute)
	{
		_next = next;
		_getAuthRoute = getAuthRoute;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		var end = context.GetEndpoint();
		var attr = (AuthPermissionAttribute?)end?.Metadata.FirstOrDefault(x => x is AuthPermissionAttribute);
		Console.WriteLine("Running auth middleware");
		Console.WriteLine(attr);
		if (attr != null)
		{
			string? username = null;
			if (context.Request.Headers.TryGetValue("authorization", out StringValues authHeaders))
			{						
				foreach (var header in authHeaders)
				{
					if (header?.StartsWith("User") == false)
						continue;
					username = header?[(header.IndexOf(" ") + 1)..];
					break;
				}
			}
			if (username != null)
			{
				Console.WriteLine(username);
				Console.WriteLine(attr.Permission);
				string route = _getAuthRoute(username, attr.Permission);
				if (await AuthServerClient.Instance.CheckAsync(route))
				{
					await _next(context);
					return;
				}
			}
			context.Response.StatusCode = 401;
			return;
		}
		else
			await _next(context);
	}
}
```

![Decorator_Proxy_CoR](https://github.com/user-attachments/assets/513bfe88-30f8-4165-8231-ae6dd73487a2)

Middleware является декоратором для самого себя, позволяя добавлять дополнительную обработку к запросу.

## Поведенческие шаблоны

### Chain of Responsibility

```csharp
public class AuthMiddleware
{
	public delegate string GetAuthRouteType(string username, string permission);

	protected readonly GetAuthRouteType _getAuthRoute;
	protected readonly RequestDelegate _next;

	public AuthMiddleware(RequestDelegate next, GetAuthRouteType getAuthRoute)
	{
		_next = next;
		_getAuthRoute = getAuthRoute;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		var end = context.GetEndpoint();
		var attr = (AuthPermissionAttribute?)end?.Metadata.FirstOrDefault(x => x is AuthPermissionAttribute);
		Console.WriteLine("Running auth middleware");
		Console.WriteLine(attr);
		if (attr != null)
		{
			string? username = null;
			if (context.Request.Headers.TryGetValue("authorization", out StringValues authHeaders))
			{						
				foreach (var header in authHeaders)
				{
					if (header?.StartsWith("User") == false)
						continue;
					username = header?[(header.IndexOf(" ") + 1)..];
					break;
				}
			}
			if (username != null)
			{
				Console.WriteLine(username);
				Console.WriteLine(attr.Permission);
				string route = _getAuthRoute(username, attr.Permission);
				if (await AuthServerClient.Instance.CheckAsync(route))
				{
					await _next(context);
					return;
				}
			}
			context.Response.StatusCode = 401;
			return;
		}
		else
			await _next(context);
	}
}
```

![Decorator_Proxy_CoR](https://github.com/user-attachments/assets/513bfe88-30f8-4165-8231-ae6dd73487a2)

Middleware реализует паттерн Chain Of Responsibility, позволяя нескольким Middleware обрабатывать запрос или передавать его далее.

### Iterator

```csharp
public class AuthMiddleware
{
	public delegate string GetAuthRouteType(string username, string permission);

	protected readonly GetAuthRouteType _getAuthRoute;
	protected readonly RequestDelegate _next;

	public AuthMiddleware(RequestDelegate next, GetAuthRouteType getAuthRoute)
	{
		_next = next;
		_getAuthRoute = getAuthRoute;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		var end = context.GetEndpoint();
		var attr = (AuthPermissionAttribute?)end?.Metadata.FirstOrDefault(x => x is AuthPermissionAttribute);
		Console.WriteLine("Running auth middleware");
		Console.WriteLine(attr);
		if (attr != null)
		{
			string? username = null;
			if (context.Request.Headers.TryGetValue("authorization", out StringValues authHeaders))
			{						
				foreach (var header in authHeaders)
				{
					if (header?.StartsWith("User") == false)
						continue;
					username = header?[(header.IndexOf(" ") + 1)..];
					break;
				}
			}
			if (username != null)
			{
				Console.WriteLine(username);
				Console.WriteLine(attr.Permission);
				string route = _getAuthRoute(username, attr.Permission);
				if (await AuthServerClient.Instance.CheckAsync(route))
				{
					await _next(context);
					return;
				}
			}
			context.Response.StatusCode = 401;
			return;
		}
		else
			await _next(context);
	}
}
```

![Iterator](https://github.com/user-attachments/assets/883e456a-6248-4672-9754-2632d4d80425)

В коде AuthMiddleware используется паттерн итератора (IEnumerator) через языковую конструкцию foreach.

### Template method

```csharp
public class DatabaseContext : DbContext
{
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Filial>(b =>
		{
			b.HasKey(x => x.ID);
			b.Property(x => x.Name).IsRequired().IsUnicode();
			b.ToTable("filials");
		});

		modelBuilder.Entity<HousingType>(b =>
		{
			b.HasKey(x => x.ID);
			b.Property(x => x.Name).IsRequired().IsUnicode();
			b.ToTable("housing_types");
		});

		modelBuilder.Entity<Settlement>(b =>
		{
			b.HasKey(x => x.ID);
			b.Property(x => x.Name).IsRequired().IsUnicode();
			b.HasOne(x => x.Filial).WithMany(x => x.Settlements);
			b.ToTable("settlements");
		});

		modelBuilder.Entity<Teo>(b =>
		{
			b.HasKey(x => x.ID);
			b.Property(x => x.Name).IsRequired().IsUnicode();
			b.Property(x => x.Author).IsRequired().IsUnicode();
			b.Property(x => x.DateCreated).IsRequired();
			b.Property(x => x.DateUpdated).IsRequired();

			b.HasOne(x => x.Filial).WithMany();
			b.HasMany(x => x.Settlements).WithMany();
			b.HasOne(x => x.HousingType).WithMany();
			b.ToTable("teos");
		});
	}
}
```

![Диаграмма без названия-TemplateMethod](https://github.com/user-attachments/assets/16988e8e-dbbb-4588-8c61-8990c11be33d)

В процессе инициализации сессии БД в entity framework вызывается шаблонный метод OnModelCreating, в котором настраивается структура модели данных.

### Strategy

```csharp
public class AuthMiddleware
{
	public delegate string GetAuthRouteType(string username, string permission);

	protected readonly GetAuthRouteType _getAuthRoute;
	protected readonly RequestDelegate _next;

	public AuthMiddleware(RequestDelegate next, GetAuthRouteType getAuthRoute)
	{
		_next = next;
		_getAuthRoute = getAuthRoute;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		var end = context.GetEndpoint();
		var attr = (AuthPermissionAttribute?)end?.Metadata.FirstOrDefault(x => x is AuthPermissionAttribute);
		Console.WriteLine("Running auth middleware");
		Console.WriteLine(attr);
		if (attr != null)
		{
			string? username = null;
			if (context.Request.Headers.TryGetValue("authorization", out StringValues authHeaders))
			{						
				foreach (var header in authHeaders)
				{
					if (header?.StartsWith("User") == false)
						continue;
					username = header?[(header.IndexOf(" ") + 1)..];
					break;
				}
			}
			if (username != null)
			{
				Console.WriteLine(username);
				Console.WriteLine(attr.Permission);
				string route = _getAuthRoute(username, attr.Permission);
				if (await AuthServerClient.Instance.CheckAsync(route))
				{
					await _next(context);
					return;
				}
			}
			context.Response.StatusCode = 401;
			return;
		}
		else
			await _next(context);
	}
}
```

![Strategy](https://github.com/user-attachments/assets/7796bf0b-c1f2-4351-b577-3249826b195a)

Паттерн Strategy используется в AuthMiddleware. При создании объекта в него передаётся алгоритм формирования пути к серверу авторизации.
