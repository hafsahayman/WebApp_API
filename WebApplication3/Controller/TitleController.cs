using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Context;
using WebApplication3.Model;
using WebApplication3.Service;
using WebApplication3.View;

namespace WebApplication3.Controller
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TitleController:ControllerBase
    {
        private readonly ITitleService _service;
        private readonly DataContext _context;
        public TitleController(ITitleService service,DataContext context)
        {
            _service = service;
            _context = context;
        }
        [HttpPost]
        public async Task<ActionResult<TitleFormViewModel>> AddTitleAsync(TitleFormViewModel viewModel)
        {
            return Ok(await _service.AddTitleAsync(viewModel));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TitleFormViewModel>>> GetAllAsync()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteAll()
        {

            //var list = await _context.Title.ToListAsync();
            //_context.Title.RemoveRange(list);
            //await _context.SaveChangesAsync();
            

            
            await _service.DeleteAllAsync();
            return NoContent();
        }

       
    }
}
