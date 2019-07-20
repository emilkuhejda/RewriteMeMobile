using System.Collections.Generic;
using RewriteMe.Mobile.ViewModels;

namespace RewriteMe.Mobile.Navigation.Parameters
{
    public class DropDownListNavigationParameters
    {
        public DropDownListNavigationParameters(IEnumerable<DropDownListViewModel> items)
        {
            Items = items;
        }

        public IEnumerable<DropDownListViewModel> Items { get; }
    }
}
