using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace ExampleAPI.Middleware
{
	[AttributeUsage(AttributeTargets.Method)]
	public class AuthPermissionAttribute : Attribute
	{
		public string Permission { get; }
		public AuthPermissionAttribute(string permission)
		{
			Permission = permission;
		}
	}

	// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
	public class AuthMiddleware
	{
		public delegate string GetAuthRouteType(string username, string permission);

		protected readonly GetAuthRouteType _getAuthRoute;
		protected readonly HttpClient _httpClient;
		protected readonly RequestDelegate _next;

		public AuthMiddleware(RequestDelegate next, GetAuthRouteType getAuthRoute)
		{
			_next = next;
			_getAuthRoute = getAuthRoute;
			_httpClient = new HttpClient();
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
					var resp = await _httpClient.GetAsync(route);
					if (resp.IsSuccessStatusCode)
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

	// Extension method used to add the middleware to the HTTP request pipeline.
	public static class AuthMiddlewareExtensions
	{
		public static IApplicationBuilder UseAuthMiddleware(this IApplicationBuilder builder, AuthMiddleware.GetAuthRouteType getAuthRoute)
		{
			return builder.UseMiddleware<AuthMiddleware>(getAuthRoute);
		}
	}
}
