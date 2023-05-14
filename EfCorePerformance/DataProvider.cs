using Microsoft.EntityFrameworkCore;

namespace EfCorePerformance;

public class DataProvider
{
    private readonly ApplicationDbContext dbContext;

    public DataProvider(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<ResponseModel> GetHumansWithLinqMethodSyntax(int pageNumber, int pageSize, CancellationToken token)
    {
        var source = dbContext.Person;

        var count = await source.CountAsync(token);
        var items = await source
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize).ToListAsync(token);

        return new ResponseModel
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecord = count,
            Data = items
        };
    }

    public async Task<ResponseModel> GetHumansWithLinqRawQuery(int pageNumber, int pageSize, CancellationToken token)
    {
        var source = dbContext.Person;

        var count = await source.CountAsync(token);

        var data = dbContext.Person
            .FromSqlRaw(@$" SELECT p.""Id"", p.""Address"", 
                            p.""Age"", p.""Name""
                            FROM ""Person"" AS p
                            LIMIT {pageSize} OFFSET {pageNumber}").ToList();

        return new ResponseModel
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecord = count,
            Data = data
        };
    }
}
