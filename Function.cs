using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Runtime.Internal;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Lambda.OpenId.Authorizer
{
    public class Function
    {

        public static readonly string Issuer = Environment.GetEnvironmentVariable("ISSUER");
        public static readonly string Audience = Environment.GetEnvironmentVariable("AUDIENCE");
        public static readonly string OpenIdConfig = Environment.GetEnvironmentVariable("OPENIDCONFIG");

        private readonly ConfigurationManager<OpenIdConnectConfiguration> _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(OpenIdConfig, new OpenIdConnectConfigurationRetriever());

        public async Task<APIGatewayCustomAuthorizerResponse> FunctionHandler(APIGatewayCustomAuthorizerRequest authEvent, ILambdaContext context)
        {
            try
            {
                (bool authorized, string username) = await CheckAuthorization(authEvent.AuthorizationToken);

                return new APIGatewayCustomAuthorizerResponse
                {
                    PrincipalID = username,
                    PolicyDocument = new APIGatewayCustomAuthorizerPolicy
                    {
                        Version = "2012-10-17",
                        Statement = new AutoConstructedList<APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement>
                        {
                            new APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement
                            {
                                Action = new HashSet<string> {"execute-api:Invoke"},
                                Resource = new HashSet<string> {authEvent.MethodArn},
                                Effect = authorized ? "Allow" : "Deny"
                            }
                        }
                    }
                };

            }
            catch (Exception e)
            {
                Console.WriteLine("Error authorizing request. " + e.Message);
                throw;
            }

        }
        public virtual async Task<(bool, string)> CheckAuthorization(string token)
        {
            var configuration = await _configurationManager.GetConfigurationAsync(CancellationToken.None);
            var validationParameters =
                new TokenValidationParameters
                {
                    ValidIssuer = Issuer,
                    ValidAudience = Audience,
                    IssuerSigningKeys = configuration.SigningKeys
                };

            try
            {
                var user = new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out SecurityToken _);

                return (true, user.Identity.Name);
            }
            catch(Exception e)
            {
                Console.WriteLine("Error authorizing request. " + e.Message);
                return (false, string.Empty); ;
            }
        }

    }
}




