using Actionstep.API.WebClient.Domain_Models;
using Actionstep.API.WebClient.Paging;
using Actionstep.API.WebClient.View_Models;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Actionstep.API.WebClient.Pages
{
    public partial class Filenotes
    {
        [Inject] private AppState _appState { get; set; }
        [Inject] private ActionstepApi _api { get; set; }
        [Inject] private IToastService _toastService { get; set; }

        private EditContext filenoteEditContext;
        private EditContext multiDeleteEditContext;

        private FilenotesViewModel ViewModel { get; set; }


        protected override void OnInitialized()
        {
            _appState.OnFilenoteResthookReceived += OnFilenoteResthookReceived;

            ViewModel = new FilenotesViewModel();
            multiDeleteEditContext = new EditContext(ViewModel);
            base.OnInitialized();
        }


        protected override async Task OnInitializedAsync()
        {
            await PageNumberChanged(1);
            await base.OnInitializedAsync();
        }


        private async Task PageNumberChanged(int changedPageNumber)
        {
            ViewModel.Loading = true;
            ViewModel.EnableMultipleDelete = false;
            ViewModel.PageNumber = changedPageNumber;

            var filenotesDto = await _api.GetFilenotesAsync(ViewModel.PageNumber, ViewModel.PageSize, ViewModel.FilterByMatter);

            ViewModel.FilenotePagedData = new Page<FilenoteViewModel>(filenotesDto.PageMetaDataDto.PageNumber, filenotesDto.PageMetaDataDto.PageSize,
                                                                      filenotesDto.PageMetaDataDto.PageCount, filenotesDto.PageMetaDataDto.RecordCount,
                                                                      ConversionFactory.ConvertToDomainModel(filenotesDto));
            ViewModel.Loading = false;
        }


        private async Task PageSizeChanged(int changedPageSize)
        {
            ViewModel.PageSize = changedPageSize;            
            await PageNumberChanged(1);
        }


        public async Task AddNewFilenote()
        {
            if (ViewModel.MatterLookupList == null)
                ViewModel.MatterLookupList = ConversionFactory.ConvertToDomainModel(await _api.GetMatterLookupAsync());

            ViewModel.AddEditViewModel = new FilenoteAddEditViewModel();
            filenoteEditContext = new EditContext(ViewModel.AddEditViewModel);

            ViewModel.ShowNewFilenoteCard = !ViewModel.ShowNewFilenoteCard;
        }


        public void CancelNewFilenote()
        {
            ViewModel.ShowNewFilenoteCard = !ViewModel.ShowNewFilenoteCard;
        }


        public async Task EditFilenote(int filenoteId)
        {
            if (ViewModel.MatterLookupList == null)
                ViewModel.MatterLookupList = ConversionFactory.ConvertToDomainModel(await _api.GetMatterLookupAsync());

            var filenote = await _api.GetFilenoteAsync(filenoteId);
            if (filenote != null)
            {
                ViewModel.AddEditViewModel = new FilenoteAddEditViewModel()
                {
                    FilenoteId = filenote.FilenoteId,
                    ActionId = filenote.Links.MatterId?.ToString(),
                    Content = filenote.Content
                };

                filenoteEditContext = new EditContext(ViewModel.AddEditViewModel);
                ViewModel.ShowNewFilenoteCard = !ViewModel.ShowNewFilenoteCard;
            }
        }


        public async Task DeleteFilenote(int filenoteId)
        {
            if (await _api.DeleteFilenoteAsync(filenoteId))
            {
                // Refresh the page the deleted file note was on.
                // Need to handle the situation when the file note deleted was the last item on page.
                await PageNumberChanged(ViewModel.PageNumber);
            }
        }


        public void CanMultipleDelete(int filenoteId, ChangeEventArgs e)
        {
            ViewModel.FilenotePagedData.DataCollection.Single(x => x.FilenoteId == filenoteId).Selected = Convert.ToBoolean(e.Value);
            ViewModel.EnableMultipleDelete = ViewModel.FilenotePagedData.DataCollection.Count(x => x.Selected) > 0;
        }


        public async Task DeleteFilenotes()
        {
            var filenotesToDelete = ViewModel.FilenotePagedData.DataCollection.Where(x => x.Selected).Select(x => x.FilenoteId);
            await _api.DeleteMultipleFilenotesAsync(filenotesToDelete);

            ViewModel.EnableMultipleDelete = false;

            await PageNumberChanged(ViewModel.PageNumber);
        }


        public async Task SearchFilenotes(string searchString)
        {
            if (searchString.Length > 2)
            {
                ViewModel.Loading = true;
                ViewModel.EnableMultipleDelete = false;
                ViewModel.PageNumber = 1;

                var filenotesDto = await _api.SearchFilenotesAsync(searchString, ViewModel.PageSize);

                ViewModel.FilenotePagedData = new Page<FilenoteViewModel>(filenotesDto.PageMetaDataDto.PageNumber, filenotesDto.PageMetaDataDto.PageSize,
                                                                          filenotesDto.PageMetaDataDto.PageCount, filenotesDto.PageMetaDataDto.RecordCount,
                                                                          ConversionFactory.ConvertToDomainModel(filenotesDto));
                ViewModel.Loading = false;
            }
            else
            {
                await PageNumberChanged(1);
            }
        }


        public async Task SelectedMatterFilterChanged(int matterId)
        {
            ViewModel.FilterByMatter = matterId;
            await PageNumberChanged(1);
        }


        public async Task SaveFilenote()
        {
            ViewModel.Debounce = true;

            if (filenoteEditContext.Validate())
            {
                bool success;
                if (ViewModel.AddEditViewModel.FilenoteId == 0)
                {
                    success = await _api.CreateFilenoteAsync(Convert.ToInt32(ViewModel.AddEditViewModel.ActionId), ViewModel.AddEditViewModel.Content);
                }
                else
                {
                    success = await _api.UpdateFilenoteAsync(ViewModel.AddEditViewModel.FilenoteId, ViewModel.AddEditViewModel.Content);
                }

                if (success)
                {
                    ViewModel.ShowNewFilenoteCard = !ViewModel.ShowNewFilenoteCard;
                    await PageNumberChanged(1);
                }
            }

            ViewModel.Debounce = false;
        }


        private void OnFilenoteResthookReceived(FilenoteResthookResponseData e)
        {
            _toastService.ShowInfo(BuildToastMessage(e), "New Filenote");
        }


        private RenderFragment BuildToastMessage(FilenoteResthookResponseData data)
        {
            RenderFragment toastContent;

            toastContent = x =>
            {
                x.OpenElement(0, "div");
                x.AddContent(1, $"Created By: {data.CreatedBy}");
                x.CloseElement();

                x.OpenElement(2, "div");
                x.AddContent(3, $"Created: {data.Created:ddd dd-MMM-yyyy HH:mm}");
                x.CloseElement();

                x.OpenElement(4, "div");
                x.AddContent(5, $"Content: {data.Content}");
                x.CloseElement();
            };

            return toastContent;
        }
    }
}