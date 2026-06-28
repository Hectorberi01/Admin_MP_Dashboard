using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;
using Admin_MP_Dashboard.Services;

namespace Admin_MP_Dashboard.Api;

/// <summary>
/// Ajoute « Authorization: Bearer {token} » sur toutes les routes admin,
/// SAUF la connexion (/admin/auth/login). En cas de réponse 401, vide la
/// session et renvoie vers /login (jeton absent, invalide ou expiré).
/// </summary>
public class BearerAuthHandler : DelegatingHandler
{
    private const string LoginPath = "/admin/auth/login";

    private readonly AuthState _auth;
    private readonly NavigationManager _nav;

    public BearerAuthHandler(AuthState auth, NavigationManager nav)
    {
        _auth = auth;
        _nav = nav;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var path = request.RequestUri?.AbsolutePath ?? string.Empty;
        var isLogin = path.EndsWith(LoginPath, StringComparison.OrdinalIgnoreCase);

        if (!isLogin && !string.IsNullOrEmpty(_auth.AccessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _auth.AccessToken);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized && !isLogin)
        {
            await _auth.ClearAsync();
            _nav.NavigateTo("login");
        }

        return response;
    }
}
