using Maynghien.Infrastructure.Repository;
using OKR.Models.Context;
using OKR.Models.Entity;

namespace ConsumerWeightUpdate.Repository
{
    public interface IKeyResultRepository : IGenericRepository<KeyResults, OKRDBContext, ApplicationUser>
    {
        int caculatePercentKeyResults(KeyResults input);
    }
}
