using System.Collections.Generic;
using RewriteMe.Mobile.ViewModels;

namespace RewriteMe.Mobile.Navigation.Parameters
{
    public class DropDownListNavigationParameters
    {
        public DropDownListNavigationParameters(string title, IEnumerable<DropDownListViewModel> items)
        {
            Title = title;
            Items = items;
        }

        public string Title { get; }

        public IEnumerable<DropDownListViewModel> Items { get; }
    }
}
