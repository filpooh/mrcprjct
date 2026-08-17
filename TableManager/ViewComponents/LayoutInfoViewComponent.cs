using Microsoft.AspNetCore.Mvc;
using TableManager.Services;

namespace TableManager.ViewComponents
{
    public class LayoutInfoViewComponent : ViewComponent
    {
        private readonly ILayoutService _layoutService;

        public LayoutInfoViewComponent(ILayoutService layoutService)
        {
            _layoutService = layoutService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _layoutService.GetLayoutDataAsync();

            return View(model);
        }
    }
}