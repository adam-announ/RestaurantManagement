using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services
{
    public class PdfService
    {
        public byte[] GenerateFacturePdf(Facture facture, Commande commande)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Element(c => ComposeHeader(c, facture));
                    page.Content().Element(c => ComposeContent(c, facture, commande));
                    page.Footer().Element(ComposeFooter);
                });
            });

            return document.GeneratePdf();
        }

        private void ComposeHeader(IContainer container, Facture facture)
        {
            container.Column(column =>
            {
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("🍽️ RestaurantApp")
                            .FontSize(24).Bold().FontColor(Colors.Orange.Medium);
                        col.Item().Text("123 Rue de la Gastronomie");
                        col.Item().Text("75001 Paris, France");
                        col.Item().Text("Tél: 01 23 45 67 89");
                    });

                    row.RelativeItem().Column(col =>
                    {
                        col.Item().AlignRight().Text($"FACTURE N° {facture.Id:D6}")
                            .FontSize(18).Bold();
                        col.Item().AlignRight().Text($"Date: {facture.Date:dd/MM/yyyy}");
                        col.Item().AlignRight().Text($"Heure: {facture.Date:HH:mm}");
                    });
                });

                column.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Medium);
            });
        }

        private void ComposeContent(IContainer container, Facture facture, Commande commande)
        {
            container.Column(column =>
            {
                // Info Client
                if (commande.Client != null)
                {
                    column.Item().PaddingBottom(15).Column(clientCol =>
                    {
                        clientCol.Item().Text("Client:").Bold();
                        clientCol.Item().Text($"{commande.Client.Prenom} {commande.Client.Nom}");
                        if (!string.IsNullOrEmpty(commande.Client.Email))
                            clientCol.Item().Text(commande.Client.Email);
                    });
                }

                // Table
                if (commande.Table != null)
                {
                    column.Item().PaddingBottom(15).Text($"Table: {commande.Table.Numero}").Bold();
                }

                // Lignes de commande
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                    });

                    // Header
                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Orange.Medium).Padding(5)
                            .Text("Article").FontColor(Colors.White).Bold();
                        header.Cell().Background(Colors.Orange.Medium).Padding(5)
                            .Text("Qté").FontColor(Colors.White).Bold().AlignCenter();
                        header.Cell().Background(Colors.Orange.Medium).Padding(5)
                            .Text("Prix Unit.").FontColor(Colors.White).Bold().AlignRight();
                        header.Cell().Background(Colors.Orange.Medium).Padding(5)
                            .Text("Total").FontColor(Colors.White).Bold().AlignRight();
                    });

                    // Rows
                    foreach (var ligne in commande.LigneCommandes)
                    {
                        var bgColor = commande.LigneCommandes.ToList().IndexOf(ligne) % 2 == 0
                            ? Colors.White : Colors.Grey.Lighten3;

                        table.Cell().Background(bgColor).Padding(5)
                            .Text(ligne.Plat?.Nom ?? "Article");
                        table.Cell().Background(bgColor).Padding(5)
                            .Text(ligne.Quantite.ToString()).AlignCenter();
                        table.Cell().Background(bgColor).Padding(5)
                            .Text($"{ligne.PrixUnitaire:F2} €").AlignRight();
                        table.Cell().Background(bgColor).Padding(5)
                            .Text($"{(ligne.Quantite * ligne.PrixUnitaire):F2} €").AlignRight();
                    }
                });

                // Total
                column.Item().PaddingTop(20).AlignRight().Column(totalCol =>
                {
                    totalCol.Item().Row(row =>
                    {
                        row.RelativeItem().AlignRight().Text("Sous-total HT:").Bold();
                        row.ConstantItem(100).AlignRight().Text($"{(facture.MontantTotal / 1.10):F2} €");
                    });
                    totalCol.Item().Row(row =>
                    {
                        row.RelativeItem().AlignRight().Text("TVA (10%):").Bold();
                        row.ConstantItem(100).AlignRight().Text($"{(facture.MontantTotal - facture.MontantTotal / 1.10):F2} €");
                    });
                    totalCol.Item().PaddingTop(5).Row(row =>
                    {
                        row.RelativeItem().AlignRight().Text("TOTAL TTC:").Bold().FontSize(14);
                        row.ConstantItem(100).AlignRight().Text($"{facture.MontantTotal:F2} €")
                            .Bold().FontSize(14).FontColor(Colors.Orange.Medium);
                    });
                });

                // Mode de paiement
                if (!string.IsNullOrEmpty(facture.ModePaiement))
                {
                    column.Item().PaddingTop(20).Column(payCol =>
                    {
                        payCol.Item().Text($"Mode de paiement: {facture.ModePaiement}").Bold();
                        payCol.Item().Text($"Statut: {facture.Statut}")
                            .FontColor(facture.Statut == "Payée" ? Colors.Green.Medium : Colors.Red.Medium);
                    });
                }
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().PaddingTop(20).LineHorizontal(1).LineColor(Colors.Grey.Medium);
                column.Item().PaddingTop(10).AlignCenter().Text("Merci de votre visite !")
                    .FontSize(14).Italic();
                column.Item().AlignCenter().Text("À bientôt chez RestaurantApp")
                    .FontSize(10).FontColor(Colors.Grey.Medium);
            });
        }
    }
}
