using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorApp.Pages
{
  public class CategoriesModel : PageModel
    {
    public List<Category> Categories { get; set; } = new();
    public async Task OnGetAsync(int skip = 0, int take = 25)
    {
      var categoriesList = new List<Category>();

      await Task.Delay(100);
      for (int i = 0; i <= 1000; i++)
      {
        categoriesList.Add(new Category(i, $"Categoria {i}", i * 17.54M));
      }

      Categories = categoriesList
        .Skip(skip)
        .Take(take)
        .ToList();
    }
  }
}

public record Category(
       int Id,
       string Title,
       decimal Price);