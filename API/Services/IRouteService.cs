using API.Models;

namespace API.Services
{
    public interface IRouteService
    {
        Task<IEnumerable<RouteModel>> Get();
        Task<RouteModel> Get(int id);
        Task<RouteModel> Create(RouteModel model);
        Task Update(RouteModel model);
        Task Delete(int id);
    }
}
