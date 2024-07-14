using Microsoft.AspNetCore.Mvc.Rendering;
using PieShop.Models;
using System.Collections.Generic;

namespace PieShop.ViewModels
{
    public class PieCreateViewModel
    {
        public PieCreateViewModel()
        {
            Item = new Item();
            Categories = new List<Category>();
        }

        public PieCreateViewModel(Item pie, IEnumerable<Category> categories)
        {
            Item = pie;
            Categories = categories;
        }

        public Item Item { get; set; } = new Item();

        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    }
}
