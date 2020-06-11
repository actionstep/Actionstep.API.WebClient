using Actionstep.API.WebClient.Paging;
using Actionstep.API.WebClient.View_Models;
using BlazorInputFile;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Actionstep.API.WebClient.Pages
{
    public partial class Documents
    {
        [Inject] private ActionstepApi _api { get; set; }
        [Inject] private IJSRuntime jSRuntime { get; set; }

        private EditContext documentEditContext;
        private EditContext multiDeleteEditContext;

        private DocumentsViewModel ViewModel { get; set; }


        protected override void OnInitialized()
        {
            ViewModel = new DocumentsViewModel();
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

            var documentsDto = await _api.GetDocumentsAsync(ViewModel.PageNumber, ViewModel.PageSize, ViewModel.FilterByMatter);

            ViewModel.DocumentPagedData = new Page<DocumentViewModel>(documentsDto.PageMetaDataDto.PageNumber, documentsDto.PageMetaDataDto.PageSize,
                                                                      documentsDto.PageMetaDataDto.PageCount, documentsDto.PageMetaDataDto.RecordCount,
                                                                      ConversionFactory.ConvertToDomainModel(documentsDto));
            ViewModel.Loading = false;
        }


        private async Task PageSizeChanged(int changedPageSize)
        {
            ViewModel.PageSize = changedPageSize;
            await PageNumberChanged(1);
        }


        public async Task AddNewDocument()
        {
            if (ViewModel.MatterLookupList == null)
                ViewModel.MatterLookupList = ConversionFactory.ConvertToDomainModel(await _api.GetMatterLookupAsync());

            ViewModel.AddEditViewModel = new DocumentAddEditViewModel();
            documentEditContext = new EditContext(ViewModel.AddEditViewModel);

            ViewModel.ShowNewDocumentCard = !ViewModel.ShowNewDocumentCard;
        }


        public void CancelNewDocument()
        {
            ViewModel.ShowNewDocumentCard = !ViewModel.ShowNewDocumentCard;
        }


        public async Task EditDocument(int documentId)
        {
            if (ViewModel.MatterLookupList == null)
                ViewModel.MatterLookupList = ConversionFactory.ConvertToDomainModel(await _api.GetMatterLookupAsync());

            var documentDto = await _api.GetDocumentAsync(documentId);
            if (documentDto != null)
            {
                ViewModel.AddEditViewModel = new DocumentAddEditViewModel()
                {
                    DocumentId = documentDto.DocumentId,
                    Name = documentDto.Name,
                    ActionId = documentDto.Links.MatterId?.ToString()
                };

                documentEditContext = new EditContext(ViewModel.AddEditViewModel);
                ViewModel.ShowNewDocumentCard = !ViewModel.ShowNewDocumentCard;
            }
        }


        public async Task DeleteDocument(int documentId)
        {
            if (await _api.DeleteDocumentAsync(documentId))
            {
                await PageNumberChanged(ViewModel.PageNumber);
            }
        }


        public void CanMultipleDelete(int documentId, ChangeEventArgs e)
        {
            ViewModel.DocumentPagedData.DataCollection.Single(x => x.DocumentId == documentId).Selected = Convert.ToBoolean(e.Value);
            ViewModel.EnableMultipleDelete = ViewModel.DocumentPagedData.DataCollection.Count(x => x.Selected) > 0;
        }


        public async Task DeleteDocuments()
        {
            var documentsToDelete = ViewModel.DocumentPagedData.DataCollection.Where(x => x.Selected).Select(x => x.DocumentId);
            await _api.DeleteMultipleDocumentsAsync(documentsToDelete);

            ViewModel.EnableMultipleDelete = false;

            await PageNumberChanged(ViewModel.PageNumber);
        }


        public async Task SelectedMatterFilterChanged(int matterId)
        {
            ViewModel.FilterByMatter = matterId;
            await PageNumberChanged(1);
        }


        public void OnFileSelectionChanged(IFileListEntry[] files)
        {
            var file = files.FirstOrDefault();
            if (file != null)
            {
                ViewModel.AddEditViewModel.UploadFile = file;
            }
        }


        public async Task SaveDocument()
        {
            ViewModel.Debounce = true;

            if (documentEditContext.Validate())
            {
                bool success = true;
                if (ViewModel.AddEditViewModel.DocumentId == 0)
                {
                    var fileIdentifier = await _api.UploadFileAsync(ViewModel.AddEditViewModel.UploadFile);
                    if (!String.IsNullOrEmpty(fileIdentifier))
                    {
                        success = await _api.CreateDocumentAsync(ViewModel.AddEditViewModel.Name, fileIdentifier, Convert.ToInt32(ViewModel.AddEditViewModel.ActionId));
                    }
                }
                else
                {
                    success = await _api.UpdateDocumentAsync(ViewModel.AddEditViewModel.DocumentId, ViewModel.AddEditViewModel.Name, Convert.ToInt32(ViewModel.AddEditViewModel.ActionId));
                }

                if (success)
                {
                    ViewModel.ShowNewDocumentCard = !ViewModel.ShowNewDocumentCard;
                    await PageNumberChanged(1);
                }
            }

            ViewModel.Debounce = false;
        }


        public async Task DownloadDocument(int documentId)
        {
            ViewModel.IsDownloading = true;
            ViewModel.DownloadingDocumentId = documentId;

            var documentToDownload = await _api.GetDocumentAsync(documentId);

            var fileContents = await _api.DownloadFileAsync(documentToDownload.InternalFileIdentifier, documentToDownload.FileSize);
            await jSRuntime.SaveAs($"{documentToDownload.Name}{documentToDownload.Extension}", fileContents);

            ViewModel.IsDownloading = false;
            ViewModel.DownloadingDocumentId = -1;
        }
    }
}