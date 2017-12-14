using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtualTourApi.Interfaces;
using VirtualTourApi.Models;

namespace VirtualTourApi.Controllers
{
    [Route("api/[controller]")]
    public class SceneController
        : Controller
    {
        private readonly IDataRepository<SceneData> sceneRepository;

        public SceneController(IDataRepository<SceneData> sceneRepository)
        {
            this.sceneRepository = sceneRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<SceneData>> Get()
        {
            return await sceneRepository.FetchItemsAsync();
        }

        [HttpGet("{name}")]
        public async Task<SceneData> Get(string name)
        {
            var results = await sceneRepository.FindItemsAsync(sd => sd.Name == name);
            return results.SingleOrDefault();
        }
    }
}
