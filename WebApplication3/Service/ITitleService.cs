using WebApplication3.Model;
using WebApplication3.View;

namespace WebApplication3.Service
{
    public interface ITitleService
    {
       Task<TitleFormViewModel> AddTitleAsync(TitleFormViewModel title);
        Task<IEnumerable<TitleFormViewModel>> GetAllAsync();
        Task DeleteAllAsync();
        //Task<IEnumerable<Title>> DeleteAllAsync();

    }
}
