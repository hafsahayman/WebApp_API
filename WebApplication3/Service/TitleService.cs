using AutoMapper;

using WebApplication3.Context;
using WebApplication3.Model;
using WebApplication3.View;
using Microsoft.EntityFrameworkCore;

namespace WebApplication3.Service
{
    public class TitleService:ITitleService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public TitleService(DataContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TitleFormViewModel> AddTitleAsync(TitleFormViewModel title)
        {
            var titleEntity = _mapper.Map<TitleFormViewModel, Title>(title); ;
            await _context.AddAsync(titleEntity);
            await _context.SaveChangesAsync();
            return _mapper.Map<Title, TitleFormViewModel>(titleEntity);
        }

        public async Task<IEnumerable<TitleFormViewModel>> GetAllAsync()
        {
            return await _context
                .Title
                .Select(c => _mapper.Map<Title, TitleFormViewModel>(c))
                .ToListAsync();


        }


        public async Task DeleteAllAsync()
        {

            List<Title> titles = _context.Title.ToList();
            _context.Title.RemoveRange(titles);
            await _context.SaveChangesAsync();

        }
        //public async Task DeleteAsyncAll()
        //{
        //    _context.Title.Clear();
        //    _context.Title.RemoveRange(_context.Title)
        //    await _context.SaveChangesAsync();
        //}
    }
}
