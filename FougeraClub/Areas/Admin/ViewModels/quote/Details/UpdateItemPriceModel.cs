public class UpdateItemPriceModel
{
    public int QuoteId { get; set; }
    public int ItemId { get; set; }
    public int SupplierId { get; set; }
    public decimal? NewPrice { get; set; }
}