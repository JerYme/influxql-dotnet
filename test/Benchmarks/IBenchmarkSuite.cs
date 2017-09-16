using System.Threading.Tasks;

namespace Benchmarks
{
    public interface IBenchmarkSuite
    {
        Task<double> SumTheWaterLevel(string endpoint, string database);

    }
}