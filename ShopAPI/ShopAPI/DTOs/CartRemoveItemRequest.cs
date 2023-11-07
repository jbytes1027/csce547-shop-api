namespace ShopAPI.DTOs;

public record CartRemoveItemRequest
{
    public int CartId { get; set; }
    public int ItemId { get; set; }
    public int? Quantity { get; set; }
}