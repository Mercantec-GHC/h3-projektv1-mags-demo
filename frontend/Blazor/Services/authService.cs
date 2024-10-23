using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using API.Models;
using Blazored.LocalStorage;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly ILocalStorageService _localStorage;

    public AuthService(HttpClient httpClient, AuthenticationStateProvider authStateProvider, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _authStateProvider = authStateProvider;
        _localStorage = localStorage;
    }

    public async Task<bool> Login(LoginDTO loginModel)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Users/login", loginModel);
        if (response.IsSuccessStatusCode)
        {
            var loginResult = await response.Content.ReadFromJsonAsync<LoginResult>();
            if (loginResult != null && !string.IsNullOrEmpty(loginResult.Token))
            {
                await _localStorage.SetItemAsync("authToken", loginResult.Token);
                ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(loginResult.Token);
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", loginResult.Token);
                return true;
            }
        }
        return false;
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("authToken");
        ((CustomAuthStateProvider)_authStateProvider).NotifyUserLogout();
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<bool> Register(SignUpDTO registerModel)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Users/register", registerModel);
        return response.IsSuccessStatusCode;
    }
}

public class LoginResult
{
    public string Token { get; set; }
    public string Username { get; set; }
    public string Id { get; set; }
}

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public CustomAuthStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");

        if (string.IsNullOrEmpty(token))
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        var claims = ParseClaimsFromJwt(token);
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
        return new AuthenticationState(user);
    }

    public void NotifyUserAuthentication(string token)
    {
        var claims = ParseClaimsFromJwt(token);
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        NotifyAuthenticationStateChanged(authState);
    }

    public void NotifyUserLogout()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(anonymousUser));
        NotifyAuthenticationStateChanged(authState);
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var token = _tokenHandler.ReadJwtToken(jwt);
        return token.Claims;
    }
}

