using System.Net.Http.Json;
using System.Text.Json;

namespace Admin_MP_Dashboard.Api;

/// <summary>Implémentation HTTP réelle du BFF Admin (http://admin.localhost).</summary>
public sealed class HttpAdminApi : IAdminApi
{
    private readonly HttpClient _http;
    public HttpAdminApi(HttpClient http) => _http = http;

    // ---------- Auth ----------
    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var resp = await _http.PostAsJsonAsync("/admin/auth/login", request);
        resp.EnsureSuccessStatusCode();
        return ParseAuth(await resp.Content.ReadAsStringAsync());
    }

    public async Task<AuthResult> RefreshAsync(RefreshRequest request)
    {
        var resp = await _http.PostAsJsonAsync("/admin/auth/refresh", request);
        resp.EnsureSuccessStatusCode();
        return ParseAuth(await resp.Content.ReadAsStringAsync());
    }

    public Task LogoutAsync(LogoutRequest request) => _http.PostAsJsonAsync("/admin/auth/logout", request);

    // ---------- Vendeurs ----------
    public async Task<IReadOnlyList<AdminSeller>> ListSellersAsync()
        => (await _http.GetFromJsonAsync<List<AdminSeller>>("/admin/sellers")) ?? new();

    public async Task<AdminSeller?> GetSellerAsync(Guid id)
        => await _http.GetFromJsonAsync<AdminSeller>($"/admin/sellers/{id}");

    public Task ApproveKybAsync(Guid id) => _http.PostAsync($"/admin/sellers/{id}/kyb/approve", null);
    public Task RejectKybAsync(Guid id) => _http.PostAsync($"/admin/sellers/{id}/kyb/reject", null);
    public Task ActivateSellerAsync(Guid id) => _http.PostAsync($"/admin/sellers/{id}/activate", null);
    public Task SuspendSellerAsync(Guid id) => _http.PostAsync($"/admin/sellers/{id}/suspend", null);
    public Task SetCommissionRateAsync(Guid id, CommissionRateRequest request) => _http.PutAsJsonAsync($"/admin/sellers/{id}/commission-rate", request);

    // ---------- Comptes & rôles ----------
    public async Task<IReadOnlyList<AdminUser>> ListUsersAsync()
        => (await _http.GetFromJsonAsync<List<AdminUser>>("/admin/users")) ?? new();

    public async Task<AdminUser?> GetUserAsync(Guid id)
        => await _http.GetFromJsonAsync<AdminUser>($"/admin/users/{id}");

    public async Task<IReadOnlyList<AdminRole>> ListRolesAsync()
        => (await _http.GetFromJsonAsync<List<AdminRole>>("/admin/roles")) ?? new();

    public Task CreateRoleAsync(CreateRoleRequest request) => _http.PostAsJsonAsync("/admin/roles", request);
    public Task AssignRoleAsync(Guid userId, RoleRefRequest request) => _http.PostAsJsonAsync($"/admin/users/{userId}/roles", request);
    public Task SuspendUserAsync(Guid userId) => _http.PostAsync($"/admin/users/{userId}/suspend", null);
    public Task ReactivateUserAsync(Guid userId) => _http.PostAsync($"/admin/users/{userId}/reactivate", null);

    // ---------- Produits ----------
    public async Task<IReadOnlyList<AdminProduct>> ListProductsAsync()
        => (await _http.GetFromJsonAsync<List<AdminProduct>>("/admin/products")) ?? new();

    public async Task<AdminProduct?> GetProductAsync(Guid id)
        => await _http.GetFromJsonAsync<AdminProduct>($"/admin/products/{id}");

    public Task ApproveProductAsync(Guid id) => _http.PostAsync($"/admin/products/{id}/approve", null);
    public Task RejectProductAsync(Guid id) => _http.PostAsync($"/admin/products/{id}/reject", null);

    // ---------- Paiements ----------
    public async Task<IReadOnlyList<AdminPayment>> ListPaymentsAsync()
        => (await _http.GetFromJsonAsync<List<AdminPayment>>("/admin/payments")) ?? new();

    // ---------- Commandes ----------
    public async Task<IReadOnlyList<AdminOrder>> ListOrdersAsync()
        => (await _http.GetFromJsonAsync<List<AdminOrder>>("/admin/orders")) ?? new();

    // ---------- Notifications / CMS / Analytics ----------
    public async Task<IReadOnlyList<AdminNotification>> ListNotificationsAsync()
        => (await _http.GetFromJsonAsync<List<AdminNotification>>("/admin/notifications")) ?? new();

    public async Task<IReadOnlyList<AdminContent>> ListContentAsync()
        => (await _http.GetFromJsonAsync<List<AdminContent>>("/admin/content")) ?? new();

    public async Task<AdminAnalyticsReport?> GetAnalyticsAsync(string scope, string granularity, string from, string to)
        => await _http.GetFromJsonAsync<AdminAnalyticsReport>($"/admin/analytics/report?scope={Uri.EscapeDataString(scope)}&granularity={Uri.EscapeDataString(granularity)}&from={Uri.EscapeDataString(from)}&to={Uri.EscapeDataString(to)}");

    // ---------- Modération ----------
    public async Task<IReadOnlyList<AdminReview>> GetProductReviewsAsync(Guid productId)
        => (await _http.GetFromJsonAsync<List<AdminReview>>($"/admin/reviews/products/{productId}")) ?? new();

    public Task FlagReviewAsync(Guid id) => _http.PostAsync($"/admin/reviews/{id}/flag", null);
    public Task RejectReviewAsync(Guid id) => _http.PostAsync($"/admin/reviews/{id}/reject", null);
    public Task RestoreReviewAsync(Guid id) => _http.PostAsync($"/admin/reviews/{id}/restore", null);

    public async Task<IReadOnlyList<AdminDispute>> ListDisputesAsync()
        => (await _http.GetFromJsonAsync<List<AdminDispute>>("/admin/disputes")) ?? new();

    public async Task<IReadOnlyList<AdminDispute>> GetDisputesByOrderAsync(Guid orderId)
        => (await _http.GetFromJsonAsync<List<AdminDispute>>($"/admin/disputes/by-order/{orderId}")) ?? new();

    public async Task<AdminDispute?> GetDisputeAsync(Guid id)
        => await _http.GetFromJsonAsync<AdminDispute>($"/admin/disputes/{id}");

    public Task ReviewDisputeAsync(Guid id) => _http.PostAsync($"/admin/disputes/{id}/review", null);
    public Task ResolveDisputeAsync(Guid id, ResolveDisputeRequest request) => _http.PostAsJsonAsync($"/admin/disputes/{id}/resolve", request);
    public Task EscalateDisputeAsync(Guid id) => _http.PostAsync($"/admin/disputes/{id}/escalate", null);

    // ---------- Finances ----------
    public async Task<IReadOnlyList<SettlementBatch>> ListBatchesAsync()
        => (await _http.GetFromJsonAsync<List<SettlementBatch>>("/admin/settlement/batches")) ?? new();

    public async Task<SettlementBatch?> GetBatchAsync(Guid id)
        => await _http.GetFromJsonAsync<SettlementBatch>($"/admin/settlement/batches/{id}");

    public async Task<Guid> RunSettlementAsync(RunSettlementRequest request)
    {
        var resp = await _http.PostAsJsonAsync("/admin/settlement/run", request);
        resp.EnsureSuccessStatusCode();
        var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
        var id = FindString(doc.RootElement, "batchId", "id");
        return Guid.TryParse(id, out var g) ? g : Guid.Empty;
    }

    public Task MarkPayoutPaidAsync(Guid batchId, Guid payoutId, MarkPaidRequest request)
        => _http.PostAsJsonAsync($"/admin/settlement/batches/{batchId}/payouts/{payoutId}/paid", request);

    public async Task<IReadOnlyList<CommissionRule>> ListCommissionRulesAsync()
        => (await _http.GetFromJsonAsync<List<CommissionRule>>("/admin/commission-rules")) ?? new();

    public Task CreateCommissionRuleAsync(CommissionRuleRequest request) => _http.PostAsJsonAsync("/admin/commission-rules", request);
    public Task DeactivateCommissionRuleAsync(Guid id) => _http.PostAsync($"/admin/commission-rules/{id}/deactivate", null);

    // ---------- Catalogue ----------
    public async Task<IReadOnlyList<AdminCategory>> ListCategoriesAsync()
        => (await _http.GetFromJsonAsync<List<AdminCategory>>("/admin/categories")) ?? new();

    public async Task<Guid> CreateCategoryAsync(CategoryRequest request)
    {
        var resp = await _http.PostAsJsonAsync("/admin/categories", request);
        resp.EnsureSuccessStatusCode();
        return ExtractId(await resp.Content.ReadAsStringAsync(), "categoryId", "id");
    }

    public Task PublishCategoryAsync(Guid id) => _http.PostAsync($"/admin/categories/{id}/publish", null);
    public Task UnpublishCategoryAsync(Guid id) => _http.PostAsync($"/admin/categories/{id}/unpublish", null);
    public Task DeleteCategoryAsync(Guid id) => _http.DeleteAsync($"/admin/categories/{id}");

    public async Task<IReadOnlyList<AdminBrand>> ListBrandsAsync()
        => (await _http.GetFromJsonAsync<List<AdminBrand>>("/admin/brands")) ?? new();

    public async Task<Guid> CreateBrandAsync(BrandRequest request)
    {
        var resp = await _http.PostAsJsonAsync("/admin/brands", request);
        resp.EnsureSuccessStatusCode();
        return ExtractId(await resp.Content.ReadAsStringAsync(), "brandId", "id");
    }

    public Task PublishBrandAsync(Guid id) => _http.PostAsync($"/admin/brands/{id}/publish", null);
    public Task UnpublishBrandAsync(Guid id) => _http.PostAsync($"/admin/brands/{id}/unpublish", null);
    public Task DeleteBrandAsync(Guid id) => _http.DeleteAsync($"/admin/brands/{id}");

    // ---------- Plateforme ----------
    public async Task<IReadOnlyList<TaxRule>> ListTaxRulesAsync()
        => (await _http.GetFromJsonAsync<List<TaxRule>>("/admin/tax/rules")) ?? new();

    public Task CreateTaxRuleAsync(TaxRuleRequest request) => _http.PostAsJsonAsync("/admin/tax/rules", request);
    public Task DeactivateTaxRuleAsync(Guid id) => _http.PostAsync($"/admin/tax/rules/{id}/deactivate", null);

    public async Task<IReadOnlyList<Campaign>> ListCampaignsAsync()
        => (await _http.GetFromJsonAsync<List<Campaign>>("/admin/campaigns")) ?? new();

    public Task CreateCampaignAsync(CampaignRequest request) => _http.PostAsJsonAsync("/admin/campaigns", request);
    public Task ScheduleCampaignAsync(Guid id) => _http.PostAsync($"/admin/campaigns/{id}/schedule", null);
    public Task ActivateCampaignAsync(Guid id) => _http.PostAsync($"/admin/campaigns/{id}/activate", null);
    public Task EndCampaignAsync(Guid id) => _http.PostAsync($"/admin/campaigns/{id}/end", null);

    public async Task<RiskAssessment?> GetRiskAssessmentAsync(string subjectType, Guid subjectId)
        => await _http.GetFromJsonAsync<RiskAssessment>($"/admin/fraud/assessments/{subjectType}/{subjectId}");

    public async Task<IReadOnlyList<InventoryItem>> ListLowStockAsync()
        => (await _http.GetFromJsonAsync<List<InventoryItem>>("/admin/inventory/low-stock")) ?? new();

    // ---------- Helpers ----------
    private static Guid ExtractId(string json, params string[] names)
    {
        if (string.IsNullOrWhiteSpace(json)) return Guid.Empty;
        using var doc = JsonDocument.Parse(json);
        var s = FindString(doc.RootElement, names);
        return Guid.TryParse(s, out var g) ? g : Guid.Empty;
    }

    private static AuthResult ParseAuth(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new AuthResult("", "", DateTime.UtcNow, "");

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        if (root.ValueKind == JsonValueKind.Object)
            foreach (var wrap in new[] { "data", "result", "payload", "tokens" })
                if (root.TryGetProperty(wrap, out var inner) && inner.ValueKind == JsonValueKind.Object)
                    root = inner;

        var access = FindString(root, "accessToken", "access_token", "token", "jwt", "idToken") ?? "";
        var refresh = FindString(root, "refreshToken", "refresh_token") ?? "";
        var name = FindString(root, "fullName", "name", "displayName", "shopName") ?? "Administrateur";

        DateTime? exp = null;
        if (DateTime.TryParse(FindString(root, "accessTokenExpiresOnUtc", "expiresAt", "expires_at", "expiry"), out var d))
            exp = d;
        else if (FindNumber(root, out var secs, "expiresIn", "expires_in"))
            exp = DateTime.UtcNow.AddSeconds(secs);

        return new AuthResult(access, refresh, exp ?? DateTime.UtcNow.AddHours(1), name);
    }

    private static string Norm(string s) => s.Replace("_", "").Replace("-", "").ToLowerInvariant();

    private static string? FindString(JsonElement obj, params string[] names)
    {
        if (obj.ValueKind != JsonValueKind.Object) return null;
        foreach (var p in obj.EnumerateObject())
            foreach (var n in names)
                if (Norm(p.Name) == Norm(n) && p.Value.ValueKind == JsonValueKind.String)
                    return p.Value.GetString();
        return null;
    }

    private static bool FindNumber(JsonElement obj, out double value, params string[] names)
    {
        value = 0;
        if (obj.ValueKind != JsonValueKind.Object) return false;
        foreach (var p in obj.EnumerateObject())
            foreach (var n in names)
                if (Norm(p.Name) == Norm(n) && p.Value.ValueKind == JsonValueKind.Number)
                {
                    value = p.Value.GetDouble();
                    return true;
                }
        return false;
    }
}
