using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment05.Models
{
    public interface ITagRepository
    {
        Task<(Response response, int taskId)> Create(TagCreateDTO tag);
        Task<ICollection<TagDTO>> Read();
        Task<TagDTO> Read(int tagId);
        Task<Response> Update(TagUpdateDTO tag);
        Task<Response> Delete(int tagId, bool force = false);
    }
}
