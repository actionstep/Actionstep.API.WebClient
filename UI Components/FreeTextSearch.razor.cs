using Microsoft.AspNetCore.Components;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Actionstep.API.WebClient.UI_Components
{
    public partial class FreeTextSearch
    {
        [Parameter] public EventCallback<string> SearchTextChanged { get; set; }

        private string SearchText { get; set; } = string.Empty;


        public async Task SearchFor()
        {
            SearchText = CleanSearchString(SearchText);
            await SearchTextChanged.InvokeAsync(SearchText);
        }


        public async Task ClearSearch()
        {
            SearchText = String.Empty;
            await SearchTextChanged.InvokeAsync(SearchText);
        }


        private string CleanSearchString(string searchString)
        {
            var cleansedString = new StringBuilder();

            foreach (var chr in searchString.ToCharArray())
            {
                if (char.IsLetterOrDigit(chr) || char.IsWhiteSpace(chr))
                {
                    cleansedString.Append(chr);
                }
            }

            return cleansedString.ToString();
        }
    }
}