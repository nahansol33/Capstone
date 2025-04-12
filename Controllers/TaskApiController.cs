using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Capstone.Data;
using Capstone.Models;


namespace capstone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TaskApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetTasks()
        {
            var tasks = _context.TaskItems.ToList();
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public IActionResult GetTask(int id)
        {
            var task = _context.TaskItems.Find(id);
            if (task == null)
                return NotFound();
            return Ok(task);
        }

        [HttpPost]
        public IActionResult AddTask([FromBody] TaskItem task)
        {
            if (ModelState.IsValid)
            {
                _context.TaskItems.Add(task);
                _context.SaveChanges();
                return CreatedAtAction(nameof(GetTask), new { id = task.TaskId }, task);
            }
            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, [FromBody] TaskItem task)
        {
            if (id != task.TaskId)
                return BadRequest();

            _context.TaskItems.Update(task);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            var task = _context.TaskItems.Find(id);
            if (task == null)
                return NotFound();

            _context.TaskItems.Remove(task);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
