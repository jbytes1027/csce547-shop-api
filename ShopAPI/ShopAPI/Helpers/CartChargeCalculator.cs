using ShopAPI.Models;

namespace ShopAPI.Helpers
{
    public static class Calculate
    {
        // Default total calculator with premade surcharges
        public static List<Total> Totals(List<CartItem> items)
        {
            List<ISurchargeCalculator> surchargeCalculators = new()
            {
                new SalesTaxSurchargeCalculator(0.07)
            };

            return Totals(items, surchargeCalculators);
        }

        public static List<Total> Totals(List<CartItem> items, List<ISurchargeCalculator> surchargeCalculators)
        {
            List<TotalType> totalsToCalculate = new()
            {
                TotalType.BaseTotal,
                TotalType.BundleTotal,
                TotalType.TaxTotal
            };

            List<Total> totals = new();
            foreach (var totalToCalculate in totalsToCalculate)
            {
                List<ISurchargeCalculator> currSurchargeCalculators = new();
                switch (totalToCalculate)
                {
                    case TotalType.BaseTotal:
                        // No calculator to add
                        break;
                    case TotalType.BundleTotal:
                        // Add all bundle discounts
                        currSurchargeCalculators.AddRange(
                            surchargeCalculators.Where(c => c.Type == TotalType.BundleTotal)
                        );
                        break;
                    case TotalType.TaxTotal:
                        // Add taxes, and everything else
                        currSurchargeCalculators = surchargeCalculators;
                        break;
                    default:
                        break;
                }

                var bill = Bill(items, currSurchargeCalculators);
                totals.Add(new Total()
                {
                    Type = totalToCalculate,
                    Value = bill.GetTotalCost(),
                });
            }

            return totals;
        }

        public static Bill Bill(List<CartItem> items, List<ISurchargeCalculator> surchargeCalculators)
        {
            Bill bill = new()
            {
                Items = items
            };

            foreach (var surchargeCalc in surchargeCalculators)
            {
                surchargeCalc.AddCalculatedTo(bill);
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

    public class SalesTaxSurchargeCalculator : ISurchargeCalculator
    {
        public double TaxRate;

        public TotalType Type { get => TotalType.TaxTotal; }

        public SalesTaxSurchargeCalculator(double taxRate)
        {
            TaxRate = taxRate;
        }

        public void AddCalculatedTo(Bill bill)
        {
            decimal taxCharge = bill.GetTotalCost() * (decimal)TaxRate;
            Surcharge taxSurcharge = new() { Cost = taxCharge, Description = "Sales Tax" };

            bill.Surcharges.Add(taxSurcharge);
        }
    }

    public interface ISurchargeCalculator
    {
        public TotalType Type { get; }
        public void AddCalculatedTo(Bill bill);
    }

    public record Surcharge
    {
        public decimal Cost { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}