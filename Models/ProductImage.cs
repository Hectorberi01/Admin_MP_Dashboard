namespace Admin_MP_Dashboard.Models;

/// <summary>
/// Image d'un produit. Correspond à un média côté BFF
/// (POST /seller/products/{id}/media · MediaRequest).
/// En démo, l'emoji sert de visuel ; Url portera l'image réelle.
/// </summary>
public class ProductImage
{
    public string? Url { get; init; }
    public string Emoji { get; init; } = "";
    public string? AltText { get; init; }
    public bool IsPrimary { get; init; }
}
