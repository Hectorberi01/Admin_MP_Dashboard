namespace Admin_MP_Dashboard.Models;

/// <summary>
/// Catalogue curé des permissions (format « ressource.action ») proposées à
/// l'administrateur lors de la création d'un rôle. Le backend valide tout code
/// au format « ressource.action » ; cette liste sert d'aide à la saisie.
/// </summary>
public static class PermissionCatalog
{
    public static readonly IReadOnlyList<PermissionGroup> Groups = new List<PermissionGroup>
    {
        new("Comptes & accès", new[] { "users.read", "users.manage", "roles.read", "roles.manage" }),
        new("Vendeurs", new[] { "sellers.read", "sellers.approve", "sellers.suspend", "sellers.commission" }),
        new("Catalogue & produits", new[] { "catalog.read", "catalog.manage", "products.read", "products.approve", "products.reject" }),
        new("Commandes & paiements", new[] { "orders.read", "orders.manage", "payments.read", "payments.refund" }),
        new("Finances", new[] { "finance.read", "finance.manage", "settlement.run" }),
        new("Modération", new[] { "reviews.moderate", "disputes.read", "disputes.resolve" }),
        new("Croissance & contenu", new[] { "marketing.manage", "content.manage", "analytics.read", "notifications.read" }),
        new("Configuration", new[] { "tax.manage", "platform.manage" }),
    };

    public static IReadOnlyList<string> All => Groups.SelectMany(g => g.Permissions).ToList();
}

public record PermissionGroup(string Label, IReadOnlyList<string> Permissions);
