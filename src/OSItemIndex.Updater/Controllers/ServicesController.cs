using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OSItemIndex.Updater.Services;

namespace OSItemIndex.Updater.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ServicesController : Controller
    {
        private readonly IStatefulServiceRepository _serviceRepository;

        public ServicesController(IStatefulServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        [HttpGet] // GET services
        public async Task<ActionResult> GetStatus()
        {
            var status = await _serviceRepository.GetStatesAsync();
            return Ok(status);
        }

        [HttpGet("{serviceName}")] // GET services/<serviceName>
        public async Task<ActionResult> GetServiceStatus(string serviceName)
        {
            var service = _serviceRepository.Services.FirstOrDefault(a => a.ServiceName == serviceName);
            if (service == null)
            {
                return NotFound();
            }

            var status = await service.GetStateAsync();
            return Ok(status);
        }
    }
}
