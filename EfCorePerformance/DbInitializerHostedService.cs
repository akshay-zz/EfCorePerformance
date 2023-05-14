using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace EfCorePerformance;

public class DbInitializerHostedService : IHostedService
{
    public static bool DataIsReady { get; set; }

    private readonly ApplicationDbContext context;


    public DbInitializerHostedService(IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope();
        context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    public static int RecordNumber { get; set; } = 1;

    public async Task Seed(CancellationToken token)
    {
        for (int i = 1; i < 926; i++)
        {
            Console.WriteLine($"Data seed batch : {i}");

            var entities = PrepareData(20000);

            context!.BulkInsert(entities);
            await context!.SaveChangesAsync(token);

            Console.WriteLine($"Data seed batch completed: {i}");
        }

        DataIsReady = true;
    }

    private static List<HumanBeing> PrepareData(int size)
    {
        var entities = new List<HumanBeing>();
        RecordNumber++;

        for (int i = 1; i < size; i++)
        {
            entities.Add(new HumanBeing
            {
                Name = $"Test user {RecordNumber}_{i}",
                Address = $"My address {RecordNumber}_{i}",
                Age = i,
            });
        }

        return entities;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var count = await context.Person.CountAsync(cancellationToken);

        if (count < 1)
        {
            await Seed(cancellationToken);
        }

        DataIsReady = true;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
