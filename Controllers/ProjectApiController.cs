
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Capstone.Data; 
using Capstone.Models;  
 

namespace Capstone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetProjects()
        {
            var projects = _context.Projects.ToList();
            return Ok(projects);
        }

        [HttpGet("{id}")]
        public IActionResult GetProject(int id)
        {
            var project = _context.Projects.Find(id);
            if (project == null)
                return NotFound();
            return Ok(project);
        }

        [HttpPost]
        public IActionResult AddProject([FromBody] Project project)
        {
            if (ModelState.IsValid)
            {
                _context.Projects.Add(project);
                _context.SaveChanges();
                return CreatedAtAction(nameof(GetProject), new { id = project.ProjectId }, project);
            }
            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProject(int id, [FromBody] Project project)
        {
            if (id != project.ProjectId)
                return BadRequest();

            _context.Projects.Update(project);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProject(int id)
        {
            var project = _context.Projects.Find(id);
            if (project == null)
                return NotFound();

            _context.Projects.Remove(project);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
