using System.Text.Json.Serialization;

namespace Admin_MP_Dashboard.Api;

// ============================================================
// Contrats du BFF Admin (http://admin.localhost). DTO permissifs : les champs
// absents prennent leur valeur par défaut (désérialisation tolérante).
// ============================================================

// ---------- Auth ----------
public record LoginRequest(string Email, string Password, string? MfaCode = null);
public record RefreshRequest(string RefreshToken);
public record LogoutRequest(string RefreshToken);
public record AuthResult(string AccessToken, string RefreshToken, DateTime ExpiresAt, string SellerName);

// ---------- Vendeurs ----------
public class AdminSeller
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ShopName { get; set; } = "";
    public string? LogoUrl { get; set; }
    public string Status { get; set; } = "";
    public string KybStatus { get; set; } = "";
    public decimal CommissionRate { get; set; }
    public decimal Rating { get; set; }
    public int SalesCount { get; set; }
}

public record CommissionRateRequest(decimal CommissionRate);

// ---------- Modération avis ----------
public class AdminReview
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid SellerId { get; set; }
    public int Rating { get; set; }
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
    public string Status { get; set; } = "";
    public string? SellerReply { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

// ---------- Litiges ----------
public class AdminDispute
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid RaisedBy { get; set; }
    public string Type { get; set; } = "";
    public string Status { get; set; } = "";
    public string? Resolution { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public record ResolveDisputeRequest(string Resolution);

// ---------- Finances : reversements ----------
public class SettlementBatch
{
    public Guid Id { get; set; }
    public DateTime PeriodStartUtc { get; set; }
    public DateTime PeriodEndUtc { get; set; }
    public string Currency { get; set; } = "XOF";
    public decimal TotalNet { get; set; }
    public string Status { get; set; } = "";
    public DateTime CreatedAtUtc { get; set; }
    public List<SettlementPayout> Payouts { get; set; } = new();
}

public class SettlementPayout
{
    public Guid Id { get; set; }
    public Guid SellerId { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal CommissionAmount { get; set; }
    public decimal NetAmount { get; set; }
    public string Currency { get; set; } = "XOF";
    public string Status { get; set; } = "";
    public string? ProviderRef { get; set; }
    public DateTime? PaidAtUtc { get; set; }
}

public record RunSettlementRequest(DateTime PeriodStartUtc, DateTime PeriodEndUtc);
public record MarkPaidRequest(string ProviderRef);

// ---------- Finances : commissions ----------
public class CommissionRule
{
    public Guid Id { get; set; }
    public string Scope { get; set; } = "";
    public Guid? TargetId { get; set; }
    public decimal Rate { get; set; }
    public decimal FixedFee { get; set; }
    public string Currency { get; set; } = "XOF";
    public bool IsActive { get; set; }
}

public record CommissionRuleRequest(
    string Scope, Guid? TargetId, decimal Rate, decimal FixedFee, string Currency,
    decimal? MinFee, decimal? MaxFee, DateTime? EffectiveFromUtc);

// ---------- Catalogue ----------
public record CategoryRequest(string Name, Guid? ParentId, string? ImageUrl, string? AttributeSchema);
public record BrandRequest(string Name, string? LogoUrl, string? Description);

// ---------- Plateforme : taxes ----------
public class TaxRule
{
    public Guid Id { get; set; }
    public string Jurisdiction { get; set; } = "";
    public Guid? CategoryId { get; set; }
    public decimal Rate { get; set; }
    public string Type { get; set; } = "";
    public bool IsActive { get; set; }
}

public record TaxRuleRequest(string Jurisdiction, Guid? CategoryId, decimal Rate, string Type, DateTime? EffectiveFromUtc);

// ---------- Plateforme : campagnes ----------
public class Campaign
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public string Status { get; set; } = "";
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
}

public record CampaignRequest(string Name, string Type, DateTime StartUtc, DateTime EndUtc, string? TargetSegment);

// ---------- Plateforme : fraude ----------
public class RiskAssessment
{
    public Guid Id { get; set; }
    public string SubjectType { get; set; } = "";
    public Guid SubjectId { get; set; }
    public double Score { get; set; }
    public string Level { get; set; } = "";
}

public record AssessRiskRequest(string SubjectType, Guid SubjectId, Dictionary<string, double> Signals);

// ---------- Plateforme : stock ----------
public class InventoryItem
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = "";
    public int OnHand { get; set; }
    public int Reserved { get; set; }
    public int Available { get; set; }
    public int ReorderThreshold { get; set; }
    public bool IsLowStock { get; set; }
}

// ---------- Comptes & rôles ----------
public class AdminUser
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string Status { get; set; } = "";
    public bool EmailVerified { get; set; }
    public bool MfaEnabled { get; set; }
    public List<Guid> RoleIds { get; set; } = new();
    public string FullName => $"{FirstName} {LastName}".Trim();
}

public class AdminRole
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public bool IsSystem { get; set; }
    public List<string> Permissions { get; set; } = new();
}

public record CreateRoleRequest(string Name, string? Description, List<string>? Permissions);
public record RoleRefRequest(Guid RoleId);

// ---------- Catégories ----------
public class AdminCategory
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string Name { get; set; } = "";
    public string Slug { get; set; } = "";
    public string Path { get; set; } = "";
    public string Status { get; set; } = "";
    public string? ImageUrl { get; set; }
}

// ---------- Marques ----------
public class AdminBrand
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Slug { get; set; } = "";
    public string Status { get; set; } = "";
    public string? LogoUrl { get; set; }
    public string? Description { get; set; }
}

// ---------- Produits ----------
public class AdminProduct
{
    public Guid Id { get; set; }
    public Guid SellerId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    public string Name { get; set; } = "";
    public string Slug { get; set; } = "";
    public string Status { get; set; } = "";
    public string? Gtin { get; set; }
    public string? Ean { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<AdminProductVariant> Variants { get; set; } = new();
    public List<AdminProductMedia> Media { get; set; } = new();

    /// <summary>URL de l'image principale (IsPrimary) sinon la première disponible.</summary>
    public string? PrimaryImageUrl =>
        (Media.FirstOrDefault(m => m.IsPrimary) ?? Media.FirstOrDefault())?.Url;
}

public class AdminProductVariant
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = "";
    public string? Barcode { get; set; }
    public int WeightGrams { get; set; }
}

public class AdminProductMedia
{
    public Guid Id { get; set; }
    public string Url { get; set; } = "";
    public string Type { get; set; } = "";
    public bool IsPrimary { get; set; }
    public int Position { get; set; }
    public string? AltText { get; set; }
}

// ---------- Paiements ----------
public class AdminPayment
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid BuyerId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "XOF";
    public string Method { get; set; } = "";
    public string Provider { get; set; } = "";
    public string Status { get; set; } = "";
    public DateTime CreatedAtUtc { get; set; }
}

// ---------- Commandes ----------
public class AdminOrder
{
    public Guid Id { get; set; }
    public Guid BuyerId { get; set; }
    public string Status { get; set; } = "";
    public DateTime CreatedAtUtc { get; set; }
    public decimal GrandTotal { get; set; }
    public List<AdminOrderLine> Lines { get; set; } = new();
    public string Reference => "CMD-" + Id.ToString("N")[..8].ToUpperInvariant();
    public int ItemCount => Lines.Sum(l => l.Quantity);
}

public class AdminOrderLine
{
    public string Sku { get; set; } = "";
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}

// ---------- Notifications ----------
public class AdminNotification
{
    public Guid Id { get; set; }
    public Guid RecipientUserId { get; set; }
    public string Channel { get; set; } = "";
    public string Subject { get; set; } = "";
    public string Body { get; set; } = "";
    public string Status { get; set; } = "";
    public DateTime CreatedAtUtc { get; set; }
}

// ---------- CMS (contenus) ----------
public class AdminContent
{
    public Guid Id { get; set; }
    public string Type { get; set; } = "";
    public string Slot { get; set; } = "";
    public int Position { get; set; }
    public string Content { get; set; } = "";
    public string Status { get; set; } = "";
}

// ---------- Analytics ----------
public class AdminAnalyticsReport
{
    public string Scope { get; set; } = "";
    public string Granularity { get; set; } = "";
    public string FromKey { get; set; } = "";
    public string ToKey { get; set; } = "";
    public int TotalOrders { get; set; }
    public decimal TotalGmv { get; set; }
}

// ---------- KPIs (composés côté front depuis /admin/sellers) ----------
public class AdminKpis
{
    public int SellersTotal { get; set; }
    public int SellersPendingKyb { get; set; }
    public int SellersActive { get; set; }
    public decimal AverageCommissionRate { get; set; }
}
