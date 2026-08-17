using Microsoft.EntityFrameworkCore;
using TableManager.Data;
using TableManager.Models.dto;

namespace TableManager.Services
{
    public class LayoutService : ILayoutService
    {
        private readonly ApplicationDbContext _context;

        public LayoutService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<GenericDto>> GetLayoutDataAsync()
        {
            var mlCsv = await _context.MlCsv
                .Select(x => new GenericDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Csv = false
                })
                .ToListAsync();

            var fileCsvs = await _context.FileCsvs
                .Select(x => new GenericDto
                {
                    Id = x.Id,
                    Name = x.FileName,
                    Csv = true
                })
                .ToListAsync();

            mlCsv.AddRange(fileCsvs);

            return mlCsv;
        }
    }
}