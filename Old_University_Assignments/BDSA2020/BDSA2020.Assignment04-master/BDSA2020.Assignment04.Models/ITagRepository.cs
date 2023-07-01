using System.Linq;

namespace BDSA2020.Assignment04.Models
{
    public interface ITagRepository
    {
        (Response response, int tagId) Create(TagCreateDTO tag);
        IQueryable<TagDTO> Read();
        TagDTO Read(int tagId);
        Response Update(TagUpdateDTO tag);
        Response Delete(int tagId, bool force = false);
    }
}