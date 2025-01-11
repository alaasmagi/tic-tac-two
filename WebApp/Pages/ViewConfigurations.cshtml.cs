using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class ViewConfigurations : PageModel
{
    private readonly IConfigRepository _configRepository;
    public List<string> Configurations { get; set; } = new List<string>();
    public string Message { get; set; } = string.Empty;
    
    public ViewConfigurations(IConfigRepository configRepository)
    {
        _configRepository = configRepository;
    }
    
    public IActionResult OnGet(string message)
    {
        Message = message;
        Configurations = _configRepository.GetConfigurationNames();
        return Page();
    }

    [BindProperty]
    public string DeleteConfiguration { get; set; } = string.Empty;
    public IActionResult OnPostDelete()
    {
        _configRepository.DeleteConfig(DeleteConfiguration);
        return OnGet("Configuration deleted successfully");
    }
    
}