﻿using IdentityServer3.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Cw3.Handlers
{
    public class BasicAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private IEnumerable<ClaimsIdentity> identities;

        public BasicAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            // IStudentDbService serwisy do komunikacji z baza
            : base(options, logger, encoder, clock)
        {
        }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing authorization header");

            //"Authorization: Basic slamama => bajty -> jan123:sd2swd"
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var credentialsBytes = Convert.FromBase64String(authHeader.Parameter);
            var crendentials = Encoding.UTF8.GetString(credentialsBytes).Split(":");

            if (crendentials.Length != 2)
                return AuthenticateResult.Fail("Incorrect authrization header value");
            // TO DO check credentials in DB



            var claims = new[]
            {
               new Claim(ClaimTypes.NameIdentifier, "1"),
               new Claim(ClaimTypes.Name, "jan123"),
               new Claim(ClaimTypes.Role, "admin"),
               new Claim(ClaimTypes.Role, "student")
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name); // dowod. Scheme - rodzaj uwierzytelnienia
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            

            return AuthenticateResult.Success(ticket);
        }
    }
}
  

 