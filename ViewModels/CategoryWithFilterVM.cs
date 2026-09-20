using System.Collections.Generic;

namespace CinemaDashboard.ViewModels
{
    public class CategoryWithFilterVM
    {
        public string Query { get; set; }
        public IEnumerable<CategoryItem> Categories { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }

    public class CategoryItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
    }
}