namespace Admin_MP_Dashboard.Api;

// Surface du BFF Admin, découpée par domaine (= tags BFF).
public interface IAdminAuthApi
{
    Task<AuthResult> LoginAsync(LoginRequest request);
    Task<AuthResult> RefreshAsync(RefreshRequest request);
    Task LogoutAsync(LogoutRequest request);
}

public interface IAdminSellersApi
{
    Task<IReadOnlyList<AdminSeller>> ListSellersAsync();
    Task<AdminSeller?> GetSellerAsync(Guid id);
    Task ApproveKybAsync(Guid id);
    Task RejectKybAsync(Guid id);
    Task ActivateSellerAsync(Guid id);
    Task SuspendSellerAsync(Guid id);
    Task SetCommissionRateAsync(Guid id, CommissionRateRequest request);
}

public interface IAdminUsersApi
{
    Task<IReadOnlyList<AdminUser>> ListUsersAsync();
    Task<AdminUser?> GetUserAsync(Guid id);
    Task<IReadOnlyList<AdminRole>> ListRolesAsync();
    Task CreateRoleAsync(CreateRoleRequest request);
    Task AssignRoleAsync(Guid userId, RoleRefRequest request);
    Task SuspendUserAsync(Guid userId);
    Task ReactivateUserAsync(Guid userId);
}

public interface IAdminProductsApi
{
    Task<IReadOnlyList<AdminProduct>> ListProductsAsync();
    Task<AdminProduct?> GetProductAsync(Guid id);
    Task ApproveProductAsync(Guid id);
    Task RejectProductAsync(Guid id);
}

public interface IAdminPaymentsApi
{
    Task<IReadOnlyList<AdminPayment>> ListPaymentsAsync();
}

public interface IAdminOrdersApi
{
    Task<IReadOnlyList<AdminOrder>> ListOrdersAsync();
}

public interface IAdminCmsApi
{
    Task<IReadOnlyList<AdminNotification>> ListNotificationsAsync();
    Task<IReadOnlyList<AdminContent>> ListContentAsync();
    Task<AdminAnalyticsReport?> GetAnalyticsAsync(string scope, string granularity, string from, string to);
}

public interface IAdminModerationApi
{
    Task<IReadOnlyList<AdminReview>> GetProductReviewsAsync(Guid productId);
    Task FlagReviewAsync(Guid id);
    Task RejectReviewAsync(Guid id);
    Task RestoreReviewAsync(Guid id);

    Task<IReadOnlyList<AdminDispute>> ListDisputesAsync();
    Task<IReadOnlyList<AdminDispute>> GetDisputesByOrderAsync(Guid orderId);
    Task<AdminDispute?> GetDisputeAsync(Guid id);
    Task ReviewDisputeAsync(Guid id);
    Task ResolveDisputeAsync(Guid id, ResolveDisputeRequest request);
    Task EscalateDisputeAsync(Guid id);
}

public interface IAdminFinanceApi
{
    Task<IReadOnlyList<SettlementBatch>> ListBatchesAsync();
    Task<SettlementBatch?> GetBatchAsync(Guid id);
    Task<Guid> RunSettlementAsync(RunSettlementRequest request);
    Task MarkPayoutPaidAsync(Guid batchId, Guid payoutId, MarkPaidRequest request);

    Task<IReadOnlyList<CommissionRule>> ListCommissionRulesAsync();
    Task CreateCommissionRuleAsync(CommissionRuleRequest request);
    Task DeactivateCommissionRuleAsync(Guid id);
}

public interface IAdminCatalogApi
{
    Task<IReadOnlyList<AdminCategory>> ListCategoriesAsync();
    Task<Guid> CreateCategoryAsync(CategoryRequest request);
    Task PublishCategoryAsync(Guid id);
    Task UnpublishCategoryAsync(Guid id);
    Task DeleteCategoryAsync(Guid id);
    Task<IReadOnlyList<AdminBrand>> ListBrandsAsync();
    Task<Guid> CreateBrandAsync(BrandRequest request);
    Task PublishBrandAsync(Guid id);
    Task UnpublishBrandAsync(Guid id);
    Task DeleteBrandAsync(Guid id);
}

public interface IAdminPlatformApi
{
    Task<IReadOnlyList<TaxRule>> ListTaxRulesAsync();
    Task CreateTaxRuleAsync(TaxRuleRequest request);
    Task DeactivateTaxRuleAsync(Guid id);

    Task<IReadOnlyList<Campaign>> ListCampaignsAsync();
    Task CreateCampaignAsync(CampaignRequest request);
    Task ScheduleCampaignAsync(Guid id);
    Task ActivateCampaignAsync(Guid id);
    Task EndCampaignAsync(Guid id);

    Task<RiskAssessment?> GetRiskAssessmentAsync(string subjectType, Guid subjectId);
    Task<IReadOnlyList<InventoryItem>> ListLowStockAsync();
}

/// <summary>Agrégat de tous les domaines du BFF Admin.</summary>
public interface IAdminApi :
    IAdminAuthApi, IAdminSellersApi, IAdminUsersApi, IAdminProductsApi, IAdminPaymentsApi,
    IAdminOrdersApi, IAdminCmsApi, IAdminModerationApi, IAdminFinanceApi, IAdminCatalogApi, IAdminPlatformApi
{
}
