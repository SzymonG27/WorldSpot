using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RouteController : ControllerBase
    {
        private readonly IRouteService routeService;
        public RouteController(IRouteService routeService)
        {
            this.routeService = routeService;
        }

        [HttpGet]
        public async Task<IEnumerable<RouteModel>> Get()
        {
            return await routeService.Get();
        }

        [HttpGet("{id}")]
        public async Task<RouteModel> Get(int id)
        {
            return await routeService.Get(id);
        }

        [HttpPost]
        public async Task<ActionResult<RouteModel>> Post([FromBody] RouteModel model)
        {
            var newRoute = await routeService.Create(model);
            return CreatedAtAction(nameof(Get), new { id = newRoute.Id }, newRoute);
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] RouteModel model)
        {
            await routeService.Update(model);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var routeToDelete = await routeService.Get(id);
            if (routeToDelete == null) return BadRequest();
            await routeService.Delete(routeToDelete.Id);
            return NoContent();
        }
    }
}
