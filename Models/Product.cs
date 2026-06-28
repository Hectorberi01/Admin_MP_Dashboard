namespace Admin_MP_Dashboard.Models;

public class Product
{
    public Guid Id { get; init; }
    public string Sku { get; init; } = "";
    public string Name { get; init; } = "";
    public string Category { get; init; } = "";

    /// <summary>Visuel principal (emoji) — repli quand la galerie est vide.</summary>
    public string Emoji { get; init; } = "📦";

    /// <summary>Galerie : un produit peut avoir plusieurs images.</summary>
    public List<ProductImage> Images { get; init; } = new();

    /// <summary>Image principale (IsPrimary), sinon la première disponible.</summary>
    public ProductImage? PrimaryImage =>
        Images.FirstOrDefault(i => i.IsPrimary) ?? Images.FirstOrDefault();

    public int ImageCount => Images.Count;

    /// <summary>Prix effectif en XOF.</summary>
    public int PriceXof { get; init; }

    /// <summary>Prix barré (avant promo), null si pas de promo.</summary>
    public int? OldPriceXof { get; init; }

    public int Stock { get; init; }
    public double Rating { get; init; }
    public int ReviewCount { get; init; }
    public int UnitsSold { get; init; }

    /// <summary>Pourcentage de réduction calculé, null si pas de promo.</summary>
    public int? DiscountPercent =>
        OldPriceXof is > 0 && OldPriceXof > PriceXof
            ? (int)Math.Round((1 - (double)PriceXof / OldPriceXof.Value) * 100)
            : null;

    public StockStatus StockStatus => Stock switch
    {
        <= 0 => StockStatus.OutOfStock,
        < 10 => StockStatus.LowStock,
        _    => StockStatus.InStock
    };
}
