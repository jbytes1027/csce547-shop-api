using ShopAPI.Models;

namespace ShopAPI.Helpers
{
    public static class Calculate
    {
        public static Bill DefaultBill(List<CartItem> items)
        {
            Bill bill = new()
            {
                Items = items
            };

            new SalesTaxSurchargeCalculator(0.07).AddCalculatedTo(bill);
            new BulkDiscountCalculator(quantityThreshold: 10, discountPercent: 0.05).AddCalculatedTo(bill);

            return bill;
        }

    }

    public class Bill
    {
        public List<CartItem> Items { get; set; }
        public List<Surcharge> BundleSurcharges { get; set; }
        public List<Surcharge> TaxSurcharges { get; set; }

        public Bill()
        {
            Items = new();
            BundleSurcharges = new();
            TaxSurcharges = new();
        }

        public decimal GetTotalWithoutSurcharges()
        {
            decimal sumTotal = 0;
            foreach (var item in Items)
            {
                sumTotal += item.Quantity * item.Product.Price;
            }
            return Math.Round(sumTotal, 2);
        }

        public decimal GetTotalWithoutTaxes()
        {
            decimal sumTotal = 0;
            sumTotal += GetTotalWithoutSurcharges();
            foreach (var surcharge in BundleSurcharges)
            {
                sumTotal += surcharge.Cost;
            }
            return Math.Round(sumTotal, 2);
        }

        public decimal GetTotalWithTaxes()
        {
            decimal sumTotal = 0;
            sumTotal += GetTotalWithoutTaxes();
            foreach (var surcharge in TaxSurcharges)
            {
                sumTotal += surcharge.Cost;
            }
            return Math.Round(sumTotal, 2);
        }
    }

    public class SalesTaxSurchargeCalculator : ISurchargeCalculator
    {
        public double TaxRate;

        public SalesTaxSurchargeCalculator(double taxRate)
        {
            TaxRate = taxRate;
        }

        public void AddCalculatedTo(Bill bill)
        {
            decimal taxCharge = bill.GetTotalWithTaxes() * (decimal)TaxRate;
            Surcharge taxSurcharge = new() { Cost = taxCharge, Description = "Sales Tax" };

            bill.TaxSurcharges.Add(taxSurcharge);
        }
    }

    public class BulkDiscountCalculator : ISurchargeCalculator
    {
        public int QuantityThreshold;
        public double DiscountPercent;

        public BulkDiscountCalculator(int quantityThreshold, double discountPercent)
        {
            QuantityThreshold = quantityThreshold;
            DiscountPercent = discountPercent;
        }

        public void AddCalculatedTo(Bill bill)
        {
            foreach (var item in bill.Items)
            {
                if (item.Quantity >= QuantityThreshold)
                {
                    decimal discount = -1 * item.Quantity * item.Product.Price * (decimal)DiscountPercent;

                    bill.BundleSurcharges.Add(
                        new Surcharge()
                        {
                            Cost = discount,
                            Description = $"{DiscountPercent:N2}% Bulk Discount on {item.Product.Name}"
                        }
                    );
                }
            }
        }
    }

    // Interface for adding charges to a bill
    public interface ISurchargeCalculator
    {
        public void AddCalculatedTo(Bill bill);
    }

    // Stores a fee (positive) or discount (negative)
    public record Surcharge
    {
        public decimal Cost { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}