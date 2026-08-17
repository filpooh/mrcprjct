using TableManager.Models.dto;

namespace TableManager.Services
{
    public interface ILayoutService
    {
        Task<List<GenericDto>> GetLayoutDataAsync();
    }
}