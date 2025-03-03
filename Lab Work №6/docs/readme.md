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

### Decorator

middleware добавляет функции
интерфейс by-convention (есть также явный интерфейс)

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

## Поведенческие шаблоны

### Chain of Responsibility

next

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

### Iterator

foreach

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

### Template method

OnModelCreating

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

### Strategy

getAuthRoute

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
