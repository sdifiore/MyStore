using Microsoft.AspNetCore.Components.Authorization;

using System.Security.Claims;

namespace BlazorApp.Services
{
	public class CustomAuthStateProvider : AuthenticationStateProvider
	{
		public override Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var user = new ClaimsPrincipal(new ClaimsIdentity()); // No user is authenticated by default

			return Task.FromResult(new AuthenticationState(user));
		}
		public void NotifyAuthenticationStateChanged(AuthenticationState state)
		{
			NotifyAuthenticationStateChanged(Task.FromResult(state));
		}
	}
}
