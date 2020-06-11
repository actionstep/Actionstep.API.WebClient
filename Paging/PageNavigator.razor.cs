using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Actionstep.API.WebClient.Paging
{
    public partial class PageNavigator
    {
        [Parameter] public int PageNumber { get; set; }
        [Parameter] public int PageSize { get; set; }
        [Parameter] public int TotalPageCount { get; set; }
        [Parameter] public int TotalRecordCount { get; set; }
        [Parameter] public EventCallback<int> PageNumberChanged { get; set; }
        [Parameter] public EventCallback<int> PageSizeChanged { get; set; }

        private List<int> AvailablePageSizes = new List<int>() { 5, 10, 15, 20, 25, 50, 100 };

        private int StartIndex { get; set; }
        private int FinishIndex { get; set; }


        protected override void OnParametersSet()
        {
            StartIndex = Math.Max(PageNumber - TotalPageCount, 1);
            FinishIndex = Math.Min(PageNumber + TotalPageCount, TotalPageCount);

            if (FinishIndex > 10)
                FinishIndex = 10;

            base.OnParametersSet();
        }


        private async Task OnPageNumberChanged(int pageNumber)
        {
            await PageNumberChanged.InvokeAsync(pageNumber);
        }


        private async Task OnPageSizeChanged(ChangeEventArgs args)
        {
            if (args.Value != null)
            {
                await PageSizeChanged.InvokeAsync(Convert.ToInt32(args.Value));
            }
        }


        private async Task OnNextPageClicked()
        {
            if (PageNumber < TotalPageCount)
            {
                await OnPageNumberChanged(PageNumber + 1);
            }
        }


        private async Task OnPreviousPageClicked()
        {
            if (PageNumber > 1)
            {
                await OnPageNumberChanged(PageNumber - 1);
            }
        }
    }
}