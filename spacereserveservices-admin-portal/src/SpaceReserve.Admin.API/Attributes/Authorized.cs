using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using log4net;
using System.Reflection;

namespace SpaceReserve.Admin.API.Attributes
{
    public class Authorized : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string[] _roles;
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(Authorized));
        public Authorized(params string[] roles)
        {
            _roles = roles;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            var keycloakSettings = configuration.GetSection("Keycloak");
            var metadataAddress = keycloakSettings["MetadataAddress"];
            var authority = keycloakSettings["Authority"];
            var audience = keycloakSettings["Audience"];

            var httpContext = context.HttpContext;
            var token = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            try
            {
                var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                    metadataAddress,
                    new OpenIdConnectConfigurationRetriever());

                var config = await configManager.GetConfigurationAsync(CancellationToken.None);

                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = authority,
                    ValidAudience = audience,
                    IssuerSigningKeys = config.SigningKeys
                };

                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                var jwtToken = validatedToken as JwtSecurityToken;
                if (jwtToken == null)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                var username = jwtToken.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
                var fullName = jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;

                var roles = jwtToken.Claims
                                    .Where(c => c.Type == "roles" || c.Type == ClaimTypes.Role)
                                    .Select(c => c.Value)
                                    .ToList();

                httpContext.Items["UserId"] = userId;
                httpContext.Items["Email"] = email;
                httpContext.Items["Username"] = username;
                httpContext.Items["FullName"] = fullName;
                httpContext.Items["Roles"] = roles;

                if (_roles.Length > 0 && !roles.Any(r => _roles.Contains(r)))
                {
                    context.Result = new ForbidResult();
                }
            }
            catch (SecurityTokenException e)
            {
                _logger.Error($"Token validation failed: {e.Message}");
                context.Result = new UnauthorizedResult();
            }
        }
    }
}