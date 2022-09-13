using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class RouteService : IRouteService
    {
        private readonly ApplicationDbContext dbContext;
        public RouteService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<RouteModel>> Get()
        {
            return await dbContext.Routes.ToListAsync();
        }

        public async Task<RouteModel> Get(int id)
        {
            return await dbContext.Routes.SingleOrDefaultAsync(t => t.Id == id);
        }

        public async Task<RouteModel> Create(RouteModel model)
        {
            dbContext.Routes.Add(model);
            await dbContext.SaveChangesAsync();
            return model;
        }

        public async Task Update(RouteModel model)
        {
            dbContext.Entry(model).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var RouteToDelete = await dbContext.Routes.FindAsync(id);
            if (RouteToDelete != null)
            {
                dbContext.Routes.Remove(RouteToDelete);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
