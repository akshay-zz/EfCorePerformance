namespace EfCorePerformance;

public class ResponseModel
{
    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalRecord { get; set; }

    public List<HumanBeing> Data { get; set; } = new List<HumanBeing>();

    public string? Message { get; set; }
}
