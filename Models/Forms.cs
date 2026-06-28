namespace Admin_MP_Dashboard.Models;

/// <summary>Fichier image sélectionné pour l'upload (POST /seller/products multipart).</summary>
public class ProductImageUpload
{
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
    public required byte[] Content { get; init; }

    /// <summary>Aperçu data-URL (affichage avant envoi).</summary>
    public string DataUrl => $"data:{ContentType};base64,{System.Convert.ToBase64String(Content)}";
}

/// <summary>Modèle de saisie pour la création d'un produit.</summary>
public class ProductForm
{
    public string Name { get; set; } = "";
    public string Category { get; set; } = "Audio";
    public string Description { get; set; } = "";

    /// <summary>Fichiers images sélectionnés (un produit peut en avoir plusieurs).</summary>
    public List<ProductImageUpload> Images { get; set; } = new();
}

/// <summary>Modèle de saisie pour la création d'une offre.</summary>
public class OfferForm
{
    public string ProductName { get; set; } = "";
    public string Sku { get; set; } = "";
    public double PriceXof { get; set; }
    public string Condition { get; set; } = "new";
    public int HandlingTime { get; set; } = 2;
}
