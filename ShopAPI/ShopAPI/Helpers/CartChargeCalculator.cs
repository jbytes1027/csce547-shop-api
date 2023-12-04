using ShopAPI.Models;

namespace ShopAPI.Helpers
{
    public static class Calculate
    {
        /// <summary>
        /// Calculate a bill based on the given items
        /// </summary>
        public static Bill DefaultBill(List<CartItem> items)
        {
            Bill bill = new()
            {
                Items = items
            };

            // Add 7% sales tax to the bill
            new SalesTaxSurchargeCalculator(0.07).AddCalculatedTo(bill);
            // Add a 5% bulk discount when ordering more than 10 items
            new BulkDiscountCalculator(quantityThreshold: 10, discountPercent: 0.05).AddCalculatedTo(bill);

            return bill;
        }

    }

    /// <summary>
    /// Stores items, charges, and taxes for a bill
    /// </summary>
    public class Bill
    {
        public List<CartItem> Items;
        public List<Surcharge> BundleSurcharges;
        public List<Surcharge> TaxSurcharges;

        public Bill()
        {
            Items = new();
            BundleSurcharges = new();
            TaxSurcharges = new();
        }

        public decimal GetTotalWithoutSurcharges()
        {
            decimal sumTotal = 0;
            // Add every price*quanity to the total
            foreach (var item in Items)
            {
                sumTotal += item.Quantity * item.Product.Price;
            }
            // Ensure 2 decimal places
            return Math.Round(sumTotal, 2);
        }

        public decimal GetTotalWithoutTaxes()
        {
            decimal sumTotal = 0;
            sumTotal += GetTotalWithoutSurcharges();
            // Add every surcharge to total
            foreach (var surcharge in BundleSurcharges)
            {
                sumTotal += surcharge.Cost;
            }
            // Ensure 2 decimal places
            return Math.Round(sumTotal, 2);
        }

        public decimal GetTotalWithTaxes()
        {
            decimal sumTotal = 0;
            sumTotal += GetTotalWithoutTaxes();
            // Add every tax to total
            foreach (var surcharge in TaxSurcharges)
            {
                sumTotal += surcharge.Cost;
            }
            // Ensure 2 decimal places
            return Math.Round(sumTotal, 2);
        }
    }

    /// <summary>
    /// Adds a sales tax surcharge to the bill
    /// </summary>
    public class SalesTaxSurchargeCalculator : ISurchargeCalculator
    {
        public double TaxRate;

        /// <param name="taxRate">The percent of sales tax to add</param>
        public SalesTaxSurchargeCalculator(double taxRate)
        {
            TaxRate = taxRate;
        }

        public void AddCalculatedTo(Bill bill)
        {
            // new tax charge = tax total before charge * tax rate
            decimal salesTaxChargeAmount = bill.GetTotalWithTaxes() * (decimal)TaxRate;
            // Create a new surcharge from the calculated tax charge
            Surcharge salesTaxSurcharge = new() { Cost = salesTaxChargeAmount, Description = "Sales Tax" };

            bill.TaxSurcharges.Add(salesTaxSurcharge);
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
            // For every item bought x or more times, add a discount for that item to the bill
            foreach (var item in bill.Items)
            {
                if (item.Quantity >= QuantityThreshold)
                {
                    // discount deduction = cost of x items bought * percent off
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