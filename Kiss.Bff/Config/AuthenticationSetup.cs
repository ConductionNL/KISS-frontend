﻿using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuthenticationSetupExtensions
    {
        private const string SignOutCallback = "/signout-callback-oidc";
        private const string CookieSchemeName = "cookieScheme";
        private const string ChallengeSchemeName = "challengeScheme";

        public static IServiceCollection AddKissAuth(this IServiceCollection services, string authority, string clientId, string clientSecret)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieSchemeName;
                options.DefaultChallengeScheme = ChallengeSchemeName;
            }).AddCookie(CookieSchemeName, options =>
            {
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.IsEssential = true;
                options.Cookie.HttpOnly = true;
                // TODO: make configurable?
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
                //options.Events.OnSigningOut = (e) => e.HttpContext.RevokeRefreshTokenAsync();
            })
            .AddOpenIdConnect(ChallengeSchemeName, options =>
            {
                options.Authority = authority;
                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
                options.SignedOutRedirectUri = SignOutCallback;

                options.ResponseType = OidcConstants.ResponseTypes.Code;
                options.UsePkce = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.Scope.Clear();
                options.Scope.Add(OidcConstants.StandardScopes.OpenId);
                options.Scope.Add(OidcConstants.StandardScopes.Profile);
                options.Scope.Add(OidcConstants.StandardScopes.OfflineAccess);
                options.SaveTokens = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = JwtClaimTypes.Name,
                    RoleClaimType = JwtClaimTypes.Role
                };

                options.Events.OnRemoteFailure = RedirectToRoot;
                options.Events.OnSignedOutCallbackRedirect = RedirectToRoot;

                options.ClaimActions.Clear();
                options.ClaimActions.MapAll();
            });

            services.AddDistributedMemoryCache();
            services.AddOpenIdConnectAccessTokenManagement();

            return services;
        }

        public static IApplicationBuilder UseStrictSameSiteExternalAuthenticationMiddleware(this IApplicationBuilder app) => app.UseMiddleware<StrictSameSiteExternalAuthenticationMiddleware>();

        public static IEndpointRouteBuilder MapKissAuthEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("api/logoff", LogoffAsync);
            endpoints.MapGet("api/me", GetMeAsync);
            endpoints.MapGet("api/challenge", ChallengeAsync);

            return endpoints;
        }

        private static Task RedirectToRoot<TOptions>(HandleRequestContext<TOptions> context) where TOptions : AuthenticationSchemeOptions
        {
            context.Response.Redirect("/");
            context.HandleResponse();

            return Task.CompletedTask;
        }

        private static async Task LogoffAsync(HttpContext httpContext)
        {
            await httpContext.SignOutAsync(CookieSchemeName);
            await httpContext.SignOutAsync(ChallengeSchemeName);
        }

        private static Task GetMeAsync(HttpContext httpContext)
        {
            var isLoggedIn = httpContext.User.Identity?.IsAuthenticated ?? false;
            var email = httpContext.User.FindFirstValue(JwtClaimTypes.Email) ?? httpContext.User.FindFirstValue(JwtClaimTypes.PreferredUserName);

            var result = new
            {
                isLoggedIn,
                email
            };
            return httpContext.Response.WriteAsJsonAsync(result, httpContext.RequestAborted);
        }

        private static Task ChallengeAsync(HttpContext httpContext)
        {
            var request = httpContext.Request;
            var returnUrl = (request.Query["returnUrl"].FirstOrDefault() ?? string.Empty)
                .AsSpan()
                .TrimStart('/');

            var fullReturnUrl = $"{request.Scheme}://{request.Host}{request.PathBase}/{returnUrl}";

            if (httpContext.User.Identity?.IsAuthenticated ?? false)
            {
                httpContext.Response.Redirect(fullReturnUrl);
                return Task.CompletedTask;
            }

            return httpContext.ChallengeAsync(new AuthenticationProperties
            {
                RedirectUri = fullReturnUrl,
            });
        }

        private class StrictSameSiteExternalAuthenticationMiddleware
        {
            private readonly RequestDelegate _next;

            public StrictSameSiteExternalAuthenticationMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task Invoke(HttpContext ctx)
            {
                var schemes = ctx.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
                var handlers = ctx.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();

                foreach (var scheme in await schemes.GetRequestHandlerSchemesAsync())
                {
                    if (await handlers.GetHandlerAsync(ctx, scheme.Name) is IAuthenticationRequestHandler handler && await handler.HandleRequestAsync())
                    {
                        // start same-site cookie special handling
                        string? location = null;
                        if (ctx.Response.StatusCode == 302)
                        {
                            location = ctx.Response.Headers["location"];
                        }
                        else if (ctx.Request.Method == "GET" && !ctx.Request.Query["skip"].Any())
                        {
                            location = ctx.Request.Path + ctx.Request.QueryString + "&skip=1";
                        }

                        if (location != null)
                        {
                            ctx.Response.ContentType = "text/html";
                            ctx.Response.StatusCode = 200;
                            var html = $@"
                        <html><head>
                            <meta http-equiv='refresh' content='0;url={location}' />
                        </head></html>";
                            await ctx.Response.WriteAsync(html);
                        }
                        // end same-site cookie special handling

                        return;
                    }
                }

                await _next(ctx);
            }
        }
    }
}
