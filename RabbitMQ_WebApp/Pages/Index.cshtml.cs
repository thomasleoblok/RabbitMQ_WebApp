using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RabbitMQ_WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

        public void OnPost()
        {
            var name = Request.Form["Name"][0];
            var email = Request.Form["Email"][0];
            var Tours = Request.Form["Tours"][0];
            var Book = Request.Form["Book"][0];
            var Cancel = Request.Form["Cancel"][0];


        }



    }
}