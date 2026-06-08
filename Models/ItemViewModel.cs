namespace GrocerySysAPI.Models
{
    public enum MeasurementUnit { Grams, Kilograms, Milliliters, Liters, Pieces }
    public enum ProductDepartment { Produce, Dairy, Bakery, Frozen, Meat, Beverages, Pantry }
    public class ItemViewModel
    {
        public string? ItemId { get; set; }
        public string? ItemName { get; set; }
        public ProductDepartment? Department { get; set; }
        public int? ItemQuantity { get; set; }

        //Size & Weight
        public double? WeightValue { get; set; }
        public MeasurementUnit? Unit { get; set; }

        // Financials
        public decimal? CostPrice { get; set; }
        public decimal? SellingPrice { get; set; }

        public string? ItemLocation { get; set; }
        public DateTime? ExpirationDate { get; set; }

    }
}
