namespace Admin_MP_Dashboard.Api;

/// <summary>Implémentation en mémoire du BFF Admin (démo, sans réseau).</summary>
public sealed class MockAdminApi : IAdminApi
{
    private static Guid G(int n) => new($"00000000-0000-0000-0000-{n:000000000000}");
    private static Task Done() => Task.CompletedTask;
    private static Task<T> Ok<T>(T v) => Task.FromResult(v);

    private readonly List<AdminSeller> _sellers = new()
    {
        new() { Id = G(1), ShopName = "AfriTech Store", Status = "Active", KybStatus = "Verified", CommissionRate = 0.10m, Rating = 4.6m, SalesCount = 128 },
        new() { Id = G(2), ShopName = "Boutique Sahel", Status = "Pending", KybStatus = "InReview", CommissionRate = 0.12m, Rating = 0m, SalesCount = 0 },
        new() { Id = G(3), ShopName = "Dakar Mode", Status = "Suspended", KybStatus = "Verified", CommissionRate = 0.10m, Rating = 3.9m, SalesCount = 42 },
    };

    private readonly List<CommissionRule> _rules = new()
    {
        new() { Id = G(50), Scope = "Global", Rate = 0.10m, FixedFee = 0m, Currency = "XOF", IsActive = true },
    };

    private readonly List<TaxRule> _tax = new()
    {
        new() { Id = G(60), Jurisdiction = "SN", Rate = 0.18m, Type = "VAT", IsActive = true },
    };

    public Task<AuthResult> LoginAsync(LoginRequest request) => Ok(new AuthResult("mock-token", "mock-refresh", DateTime.UtcNow.AddHours(1), "Administrateur"));
    public Task<AuthResult> RefreshAsync(RefreshRequest request) => Ok(new AuthResult("mock-token", "mock-refresh", DateTime.UtcNow.AddHours(1), "Administrateur"));
    public Task LogoutAsync(LogoutRequest request) => Done();

    public Task<IReadOnlyList<AdminSeller>> ListSellersAsync() => Ok<IReadOnlyList<AdminSeller>>(_sellers);
    public Task<AdminSeller?> GetSellerAsync(Guid id) => Task.FromResult(_sellers.FirstOrDefault(s => s.Id == id));
    public Task ApproveKybAsync(Guid id) { Set(id, s => s.KybStatus = "Verified"); return Done(); }
    public Task RejectKybAsync(Guid id) { Set(id, s => s.KybStatus = "Rejected"); return Done(); }
    public Task ActivateSellerAsync(Guid id) { Set(id, s => s.Status = "Active"); return Done(); }
    public Task SuspendSellerAsync(Guid id) { Set(id, s => s.Status = "Suspended"); return Done(); }
    public Task SetCommissionRateAsync(Guid id, CommissionRateRequest request) { Set(id, s => s.CommissionRate = request.CommissionRate); return Done(); }
    private void Set(Guid id, Action<AdminSeller> act) { var s = _sellers.FirstOrDefault(x => x.Id == id); if (s is not null) act(s); }

    public Task<IReadOnlyList<AdminUser>> ListUsersAsync() => Ok<IReadOnlyList<AdminUser>>(new List<AdminUser>
    {
        new() { Id = G(70), FirstName = "Hector", LastName = "Admin", Email = "admin@marketplace.com", Status = "Active", EmailVerified = true },
        new() { Id = G(71), FirstName = "Awa", LastName = "Diop", Email = "awa@example.com", Status = "Active", EmailVerified = true },
    });
    public Task<AdminUser?> GetUserAsync(Guid id)
        => Task.FromResult<AdminUser?>(new AdminUser { Id = id, FirstName = "Demo", LastName = "Vendeur", Email = "seller@marketplace.com", PhoneNumber = "+221770000001", Status = "Active", EmailVerified = true });
    public Task<IReadOnlyList<AdminRole>> ListRolesAsync() => Ok<IReadOnlyList<AdminRole>>(new List<AdminRole>
    {
        new() { Id = G(80), Name = "Admin", IsSystem = true, Permissions = new() { "users.manage", "roles.manage" } },
        new() { Id = G(81), Name = "Seller", IsSystem = true, Permissions = new() { "catalog.write" } },
        new() { Id = G(82), Name = "Buyer", IsSystem = true },
    });
    public Task CreateRoleAsync(CreateRoleRequest request) => Done();
    public Task AssignRoleAsync(Guid userId, RoleRefRequest request) => Done();
    public Task SuspendUserAsync(Guid userId) => Done();
    public Task ReactivateUserAsync(Guid userId) => Done();

    public Task<IReadOnlyList<AdminProduct>> ListProductsAsync() => Ok<IReadOnlyList<AdminProduct>>(new List<AdminProduct>
    {
        new() { Id = G(90), Name = "Casque audio sans fil", Slug = "casque-audio", Status = "Draft" },
        new() { Id = G(91), Name = "Montre connectée", Slug = "montre-connectee", Status = "Active" },
    });
    public Task<AdminProduct?> GetProductAsync(Guid id)
        => Task.FromResult<AdminProduct?>(new AdminProduct { Id = id, Name = "Produit", Slug = "produit", Status = "Active" });
    public Task ApproveProductAsync(Guid id) => Done();
    public Task RejectProductAsync(Guid id) => Done();

    public Task<IReadOnlyList<AdminPayment>> ListPaymentsAsync() => Ok<IReadOnlyList<AdminPayment>>(new List<AdminPayment>());

    public Task<IReadOnlyList<AdminOrder>> ListOrdersAsync() => Ok<IReadOnlyList<AdminOrder>>(new List<AdminOrder>
    {
        new() { Id = G(100), Status = "Paid", CreatedAtUtc = DateTime.UtcNow.AddHours(-3), GrandTotal = 45000m, Lines = new() { new() { Sku = "CAS-01", Quantity = 1, LineTotal = 45000m } } },
        new() { Id = G(101), Status = "Pending", CreatedAtUtc = DateTime.UtcNow.AddHours(-1), GrandTotal = 12000m, Lines = new() { new() { Sku = "MON-02", Quantity = 2, LineTotal = 12000m } } },
    });

    public Task<IReadOnlyList<AdminNotification>> ListNotificationsAsync() => Ok<IReadOnlyList<AdminNotification>>(new List<AdminNotification>
    {
        new() { Id = G(110), Channel = "Email", Subject = "Commande confirmée", Status = "Sent", CreatedAtUtc = DateTime.UtcNow.AddHours(-2) },
    });
    public Task<IReadOnlyList<AdminContent>> ListContentAsync() => Ok<IReadOnlyList<AdminContent>>(new List<AdminContent>
    {
        new() { Id = G(120), Type = "Banner", Slot = "home-hero", Position = 1, Content = "Soldes d'été", Status = "Published" },
    });
    public Task<AdminAnalyticsReport?> GetAnalyticsAsync(string scope, string granularity, string from, string to)
        => Task.FromResult<AdminAnalyticsReport?>(new AdminAnalyticsReport { Scope = scope, Granularity = granularity, FromKey = from, ToKey = to, TotalOrders = 128, TotalGmv = 5_400_000m });

    public Task<IReadOnlyList<AdminReview>> GetProductReviewsAsync(Guid productId) => Ok<IReadOnlyList<AdminReview>>(new List<AdminReview>());
    public Task FlagReviewAsync(Guid id) => Done();
    public Task RejectReviewAsync(Guid id) => Done();
    public Task RestoreReviewAsync(Guid id) => Done();

    public Task<IReadOnlyList<AdminDispute>> ListDisputesAsync() => Ok<IReadOnlyList<AdminDispute>>(new List<AdminDispute>
    {
        new() { Id = G(130), OrderId = G(100), Type = "ItemNotReceived", Status = "Open", CreatedAtUtc = DateTime.UtcNow.AddDays(-1) },
    });
    public Task<IReadOnlyList<AdminDispute>> GetDisputesByOrderAsync(Guid orderId) => Ok<IReadOnlyList<AdminDispute>>(new List<AdminDispute>());
    public Task<AdminDispute?> GetDisputeAsync(Guid id) => Task.FromResult<AdminDispute?>(null);
    public Task ReviewDisputeAsync(Guid id) => Done();
    public Task ResolveDisputeAsync(Guid id, ResolveDisputeRequest request) => Done();
    public Task EscalateDisputeAsync(Guid id) => Done();

    public Task<IReadOnlyList<SettlementBatch>> ListBatchesAsync() => Ok<IReadOnlyList<SettlementBatch>>(new List<SettlementBatch>());
    public Task<SettlementBatch?> GetBatchAsync(Guid id) => Task.FromResult<SettlementBatch?>(null);
    public Task<Guid> RunSettlementAsync(RunSettlementRequest request) => Ok(G(99));
    public Task MarkPayoutPaidAsync(Guid batchId, Guid payoutId, MarkPaidRequest request) => Done();
    public Task<IReadOnlyList<CommissionRule>> ListCommissionRulesAsync() => Ok<IReadOnlyList<CommissionRule>>(_rules);
    public Task CreateCommissionRuleAsync(CommissionRuleRequest request) { _rules.Add(new() { Id = Guid.NewGuid(), Scope = request.Scope, Rate = request.Rate, FixedFee = request.FixedFee, Currency = request.Currency, IsActive = true }); return Done(); }
    public Task DeactivateCommissionRuleAsync(Guid id) { var r = _rules.FirstOrDefault(x => x.Id == id); if (r is not null) r.IsActive = false; return Done(); }

    public Task<IReadOnlyList<AdminCategory>> ListCategoriesAsync() => Ok<IReadOnlyList<AdminCategory>>(new List<AdminCategory>
    {
        new() { Id = G(40), Name = "Électronique", Slug = "electronique", Path = "electronique", Status = "Active" },
        new() { Id = G(41), ParentId = G(40), Name = "Audio", Slug = "audio", Path = "electronique/audio", Status = "Active" },
    });
    public Task<Guid> CreateCategoryAsync(CategoryRequest request) => Ok(Guid.NewGuid());
    public Task PublishCategoryAsync(Guid id) => Done();
    public Task UnpublishCategoryAsync(Guid id) => Done();
    public Task DeleteCategoryAsync(Guid id) => Done();
    public Task<IReadOnlyList<AdminBrand>> ListBrandsAsync() => Ok<IReadOnlyList<AdminBrand>>(new List<AdminBrand>
    {
        new() { Id = G(140), Name = "Sony", Slug = "sony", Status = "Active" },
        new() { Id = G(141), Name = "Nike", Slug = "nike", Status = "Pending" },
    });
    public Task<Guid> CreateBrandAsync(BrandRequest request) => Ok(Guid.NewGuid());
    public Task PublishBrandAsync(Guid id) => Done();
    public Task UnpublishBrandAsync(Guid id) => Done();
    public Task DeleteBrandAsync(Guid id) => Done();

    public Task<IReadOnlyList<TaxRule>> ListTaxRulesAsync() => Ok<IReadOnlyList<TaxRule>>(_tax);
    public Task CreateTaxRuleAsync(TaxRuleRequest request) { _tax.Add(new() { Id = Guid.NewGuid(), Jurisdiction = request.Jurisdiction, Rate = request.Rate, Type = request.Type, IsActive = true }); return Done(); }
    public Task DeactivateTaxRuleAsync(Guid id) { var t = _tax.FirstOrDefault(x => x.Id == id); if (t is not null) t.IsActive = false; return Done(); }
    public Task<IReadOnlyList<Campaign>> ListCampaignsAsync() => Ok<IReadOnlyList<Campaign>>(new List<Campaign>());
    public Task CreateCampaignAsync(CampaignRequest request) => Done();
    public Task ScheduleCampaignAsync(Guid id) => Done();
    public Task ActivateCampaignAsync(Guid id) => Done();
    public Task EndCampaignAsync(Guid id) => Done();
    public Task<RiskAssessment?> GetRiskAssessmentAsync(string subjectType, Guid subjectId) => Task.FromResult<RiskAssessment?>(null);
    public Task<IReadOnlyList<InventoryItem>> ListLowStockAsync() => Ok<IReadOnlyList<InventoryItem>>(new List<InventoryItem>());
}
