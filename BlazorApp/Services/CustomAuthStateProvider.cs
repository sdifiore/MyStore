using System.Net.Http.Json;
using Blazored.LocalStorage;
using System.Security.Claims;
using System.Text.Json.Nodes;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorApp.Services
{
	public class CustomAuthStateProvider : AuthenticationStateProvider
	{
		private readonly HttpClient _httpClient;
		private readonly ISyncLocalStorageService _localStorage;

		public CustomAuthStateProvider(HttpClient httpClient, ISyncLocalStorageService localStorage)
		{
			_httpClient = httpClient;
			_localStorage = localStorage;

			var accessToken = _localStorage.GetItem<string>("accessToken");

			if (!string.IsNullOrEmpty(accessToken))
			{
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
			}
			else
			{
				_httpClient.DefaultRequestHeaders.Authorization = null; // Clear the header if no token is present
			}
		}

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			/*
			//var user = new ClaimsPrincipal(new ClaimsIdentity()); // Default to an empty user

			var claims = new List<Claim> { new Claim(ClaimTypes.Name, "John") };
			var identity = new ClaimsIdentity(claims, "ANY");
			var user = new ClaimsPrincipal(identity);

			return Task.FromResult(new AuthenticationState(user));
			*/

			var user = new ClaimsPrincipal(new ClaimsIdentity()); // Default to an empty non-authenticated (anonymous) user

			try
			{
				var response = await _httpClient.GetAsync("manage/info");

				if (response.IsSuccessStatusCode)
				{
					var strResponse = await response.Content.ReadAsStringAsync();
					var jsonResponse = JsonNode.Parse(strResponse);
					var email = jsonResponse?["email"]?.ToString();

					var claims = new List<Claim>
					{
						new(ClaimTypes.Name, email ?? string.Empty),
						new(ClaimTypes.Name, email ?? string.Empty),
					};

					// set the user identity
					var identity = new ClaimsIdentity(claims, "Token");
					user = new ClaimsPrincipal(identity);

					return new AuthenticationState(user);
				}
			}

			catch { }


			return (new AuthenticationState(user));
		}

		public async Task<FormResult> LoginAsync(string email, string password)
		{
			try
			{
				var response = await _httpClient.PostAsJsonAsync("login", new { email, password });

				if (response.IsSuccessStatusCode)
				{
					var strResponse = await response.Content.ReadAsStringAsync();
					var jsonResponse = JsonNode.Parse(strResponse);
					var accessToken = jsonResponse?["accessToken"]?.ToString();
					var refreshToken = jsonResponse?["refreshToken"]?.ToString();

					_localStorage.SetItem("accessToken", accessToken);
					_localStorage.SetItem("refreshToken", refreshToken);

					_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

					// Need to refresh the authentication state
					NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

					// Success!
					return new FormResult { Success = true };
				}

				else
				{
					return new FormResult { Success = false, Errors = ["Bad Email or Password."] };
				}
			}
			catch { }

			return new FormResult { Success = false, Errors = ["Connection Error]"] };
		}

		public void Logout()
		{
			_localStorage.RemoveItem("accessToken");
			_localStorage.RemoveItem("refreshToken");
			_httpClient.DefaultRequestHeaders.Authorization = null; // Clear the header
			// Notify the authentication state has changed
			NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
		}

		public class FormResult
		{
			public bool Success { get; set; }
			public string[] Errors { get; set; } = new string[0];
		}

		public void NotifyAuthenticationStateChanged(AuthenticationState state)
		{
			NotifyAuthenticationStateChanged(Task.FromResult(state));
		}
	}
}
