namespace BookShop.Data.Dtos;

public class TransactionDto
{
    public int? Id { get; set; }
    public int? ClientName { get; set; }
    public decimal Amount { get; set; }
    public DateTime? Date { get; set; }
}