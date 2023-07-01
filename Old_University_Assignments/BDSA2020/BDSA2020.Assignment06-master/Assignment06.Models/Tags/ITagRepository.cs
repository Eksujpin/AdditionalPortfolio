using System.Linq;
using System.Threading.Tasks;

namespace Assignment06.Models
{
    public interface ITagRepository
    {
        Task<(Status response, int taskId)> Create(TagCreateDTO tag);
        IQueryable<TagDTO> Read();
        Task<TagDTO> Read(int tagId);
        Task<Status> Update(TagUpdateDTO tag);
        Task<Status> Delete(int tagId, bool force = false);
    }
}
