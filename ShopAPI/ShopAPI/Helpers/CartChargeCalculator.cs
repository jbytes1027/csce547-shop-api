using NuGet.Protocol;
using ShopAPI.Models;

namespace ShopAPI.Helpers
{
    public static class Calculate
    {
        public static List<Total> Totals(List<CartItem> items)
        {
            List<Total> totals = new();
            List<TotalType> totalTypes = new();

            // Calculate Base Total
            totalTypes.Add(TotalType.BaseTotal);
            totals.Add(new()
            {
                Type = TotalType.BaseTotal,
                Value = Bill(items, totalTypes).GetTotalCost(),
            });

            // Calculate Bundle Total
            totalTypes.Add(TotalType.BundleTotal);
            totals.Add(new()
            {
                Type = TotalType.BundleTotal,
                Value = Bill(items, totalTypes).GetTotalCost(),
            });

            // Calculate Tax Total on top of Bundle Total
            totalTypes.Add(TotalType.TaxTotal);
            totals.Add(new()
            {
                Type = TotalType.TaxTotal,
                Value = Bill(items, totalTypes).GetTotalCost(),
            });

            Console.WriteLine("======== BILL ========\n" + Bill(items, totalTypes).ToJson());

            return totals;
        }

        public static Bill Bill(List<CartItem> items, List<TotalType> totals)
        {
            Bill bill = new()
            {
                Items = items
            };

            foreach (var totalType in totals)
            {
                switch (totalType)
                {
                    case TotalType.BaseTotal:
                        break;
                    case TotalType.BundleTotal:
                        break;
                    case TotalType.TaxTotal:
                        AddTaxSurcharges.To(bill);
                        break;
                    default:
                        break;
                }
            }

            return bill;
        }
    }

    public class Bill
    {
        public List<CartItem> Items;
        public List<Surcharge> Surcharges;

        public Bill()
        {
            Items = new();
            Surcharges = new();
        }

        public decimal GetTotalCost()
        {
            decimal sumTotal = 0;
            foreach (var item in Items)
            {
                sumTotal += item.Quantity * item.Product.Price;
            }
            foreach (var surcharge in Surcharges)
            {
                sumTotal += surcharge.Cost;
            }
            return sumTotal;
        }
    }

    public class AddTaxSurcharges : IAddSurcharges
    {
        private const double TaxRate = 0.07;

        public readonly TotalType type = TotalType.TaxTotal;
        public static void To(Bill bill)
        {
            decimal taxCharge = bill.GetTotalCost() * (decimal)(TaxRate);
            Surcharge taxSurcharge = new() { Cost = taxCharge, Description = "Sales Tax" };

            bill.Surcharges.Add(taxSurcharge);
        }
    }

    public interface IAddSurcharges
    {
        public static TotalType type;
        public static abstract void To(Bill bill);
    }

    public record Surcharge
    {
        public decimal Cost { get; set; }
        public string Description { get; set; } = String.Empty;
    }
}