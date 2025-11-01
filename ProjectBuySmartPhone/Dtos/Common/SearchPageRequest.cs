using System.Reflection;

namespace ProjectBuySmartPhone.Dtos.Common
{
    public class SearchPageRequest<T>
    {
        public SearchPageRequest() { }

        public int? PageIndex {  get; set; }
        public int? PageSize {  get; set; }
        public bool? IsAsc {  get; set; }
        public String? SortBy {  get; set; }

        public void ValidateInput()
        {
            if (PageIndex == null || PageIndex <= 0)
            {
                PageIndex = 1;
            }
            if (PageSize == null || PageSize <= 0)
            {
                PageSize = 10;
            }
            if(IsAsc == null)
            {
                IsAsc = false;
            }
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if (properties.Length == 0)
            {
                throw new InvalidOperationException($"Kiểu {typeof(T).Name} không có thuộc tính nào.");
            }

            if (!string.IsNullOrWhiteSpace(SortBy))
            {
                bool isValid = properties.Any(p =>
                    string.Equals(p.Name, SortBy, StringComparison.OrdinalIgnoreCase));

                if (!isValid)
                {
                    SortBy = properties[0].Name;
                }
            } else
            {
                SortBy = properties[0].Name;
            }
        }

    }
}
