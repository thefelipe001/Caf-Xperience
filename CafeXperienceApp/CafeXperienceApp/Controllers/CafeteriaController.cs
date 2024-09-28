using CafeXperienceApp.Interfaces;
using CafeXperienceApp.Models;
using CafeXperienceApp.Repositorio;
using Microsoft.AspNetCore.Mvc;

namespace CafeXperienceApp.Controllers
{
    [ApiController]
    [Route("Api/[Controller]")]
    public class CafeteriaController : Controller
    {
        private readonly ILogger<CafeteriaController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IBaseRepository<Cafeteria> _repository;

        public CafeteriaController(ILogger<CafeteriaController> logger, ApplicationDbContext db, IBaseRepository<Cafeteria> repository)
        {
            _logger = logger;
            _db = db;
            _repository = repository;

        }

        [HttpGet]
        [Route("GetCafeteria")]
        public async Task<IActionResult> GetCafeteria([FromQuery] string id)
        {
            if (!string.IsNullOrEmpty(id))
                return Ok(await _repository.GetFirstAsync(q => q.IdCafeteria == int.Parse(id)));
            return Ok(await _repository.GetAll());   
        }

        [HttpPost]
        [Route("AddCafeteria")]
        public async Task<IActionResult> AddCafeteria([FromBody] Cafeteria cafeteria)
        {
            if (cafeteria == null)
                return BadRequest();

            var success = await _repository.Add(cafeteria);

            if (success.Success)
                return Ok(cafeteria);

            return StatusCode(500, "Internal Server Error");
        }

        [HttpPut]
        [Route("UpdateCafeteria")]
        public async Task<IActionResult> UpdateCafeteria([FromBody] Cafeteria cafeteria)
        {
            if (cafeteria == null)
                return BadRequest();

            var success = await _repository.Update(cafeteria);

            if (success.Success)
                return Ok(cafeteria);

            return StatusCode(500, "Internal Server Error");
        }

        [HttpDelete]
        [Route("DeleteCafeteria")]
        public async Task<IActionResult> DeleteCafeteria([FromBody] Cafeteria cafeteria)
        {
            if (cafeteria == null)
                return BadRequest();

            var success = await _repository.Delete(cafeteria);

            if (success.Success)
                return Ok(cafeteria);

            return StatusCode(500, "Internal Server Error");
        }

    }
}
