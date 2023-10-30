using ShopAPI.DTOs;
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

            return bill;
        }

    }

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
            foreach (var item in Items)
            {
                sumTotal += item.Quantity * item.Product.Price;
            }
            return sumTotal;
        }

        public decimal GetTotalWithoutTaxes()
        {
            decimal sumTotal = 0;
            sumTotal += GetTotalWithoutSurcharges();
            foreach (var surcharge in BundleSurcharges)
            {
                sumTotal += surcharge.Cost;
            }
            return sumTotal;
        }

        public decimal GetTotalWithTaxes()
        {
            decimal sumTotal = 0;
            sumTotal += GetTotalWithoutTaxes();
            foreach (var surcharge in TaxSurcharges)
            {
                sumTotal += surcharge.Cost;
            }
            return sumTotal;
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

    public interface ISurchargeCalculator
    {
        public void AddCalculatedTo(Bill bill);
    }

    public record Surcharge
    {
        public decimal Cost { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}