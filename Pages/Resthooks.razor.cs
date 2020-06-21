using Actionstep.API.WebClient.Paging;
using Actionstep.API.WebClient.View_Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Actionstep.API.WebClient.Pages
{
    public partial class Resthooks
    {
        [Inject] private ActionstepApi _api { get; set; }

        private EditContext resthookEditContext;
        private EditContext multiDeleteEditContext;

        private ResthooksViewModel ViewModel { get; set; }


        protected override void OnInitialized()
        {
            ViewModel = new ResthooksViewModel();
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
            ViewModel.PageNumber = changedPageNumber;

            var resthooksDto = await _api.GetResthooksAsync(ViewModel.PageNumber, ViewModel.PageSize);

            ViewModel.ResthookPagedData = new Page<ResthookViewModel>(resthooksDto.PageMetaDataDto.PageNumber, resthooksDto.PageMetaDataDto.PageSize,
                                                                      resthooksDto.PageMetaDataDto.PageCount, resthooksDto.PageMetaDataDto.RecordCount,
                                                                      ConversionFactory.ConvertToDomainModel(resthooksDto));
            ViewModel.Loading = false;
        }


        private async Task PageSizeChanged(int changedPageSize)
        {
            ViewModel.PageSize = changedPageSize;
            await PageNumberChanged(1);
        }


        public void AddResthook()
        {
            ViewModel.AddEditViewModel = new ResthookAddEditViewModel();
            resthookEditContext = new EditContext(ViewModel.AddEditViewModel);

            ViewModel.ShowResthookCard = !ViewModel.ShowResthookCard;
        }


        public void CancelNewResthook()
        {
            ViewModel.ShowResthookCard = !ViewModel.ShowResthookCard;
            ViewModel.ResthookBeingEdited = 0;
        }


        public async Task EditResthook(int resthookId)
        {
            var resthook = await _api.GetResthookAsync(resthookId);
            if (resthook != null)
            {
                ViewModel.ResthookBeingEdited = resthookId;

                ViewModel.AddEditViewModel = new ResthookAddEditViewModel()
                {
                    ResthookId = resthook.ResthookId,
                    EventName = resthook.EventName,
                    TargetUrl = resthook.TargetUrl    
                };

                resthookEditContext = new EditContext(ViewModel.AddEditViewModel);
                ViewModel.ShowResthookCard = !ViewModel.ShowResthookCard;
            }
        }


        public async Task SaveResthook()
        {
            ViewModel.Debounce = true;

            if (resthookEditContext.Validate())
            {
                bool success;
                if (ViewModel.AddEditViewModel.ResthookId == 0)
                {
                    success = await _api.CreateResthookAsync(ViewModel.AddEditViewModel.EventName, ViewModel.AddEditViewModel.TargetUrl);
                }
                else
                {
                    success = await _api.UpdateResthookAsync(ViewModel.AddEditViewModel.ResthookId, ViewModel.AddEditViewModel.EventName, ViewModel.AddEditViewModel.TargetUrl);
                }

                if (success)
                {
                    ViewModel.ShowResthookCard = !ViewModel.ShowResthookCard;
                    await PageNumberChanged(1);
                }
            }

            ViewModel.ResthookBeingEdited = 0;
            ViewModel.Debounce = false;
        }


        public void CanMultipleDelete(int resthookId, ChangeEventArgs e)
        {
            ViewModel.ResthookPagedData.DataCollection.Single(x => x.ResthookId == resthookId).Selected = Convert.ToBoolean(e.Value);
            ViewModel.EnableMultipleDelete = ViewModel.ResthookPagedData.DataCollection.Count(x => x.Selected) > 0;
        }


        public async Task DeleteResthook(int resthookId)
        {
            if (await _api.DeleteResthookAsync(resthookId))
            {
                await PageNumberChanged(ViewModel.PageNumber);
            }
        }


        public async Task DeleteResthooks()
        {
            var resthooksToDelete = ViewModel.ResthookPagedData.DataCollection.Where(x => x.Selected).Select(x => x.ResthookId);
            await _api.DeleteMultipleResthooksAsync(resthooksToDelete);

            ViewModel.EnableMultipleDelete = false;

            await PageNumberChanged(ViewModel.PageNumber);
        }
    }
}