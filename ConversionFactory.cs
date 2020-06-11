using Actionstep.API.WebClient.Data_Transfer_Objects.Responses;
using Actionstep.API.WebClient.Domain_Models;
using Actionstep.API.WebClient.View_Models;
using System.Collections.Generic;
using System.Linq;

namespace Actionstep.API.WebClient
{
    public static class ConversionFactory
    {
        public static List<ResthookViewModel> ConvertToDomainModel(ResthooksResponseDto source)
        {
            if (source == null) return new List<ResthookViewModel>();
            if (source.Resthooks.Count() == 0) return new List<ResthookViewModel>();

            var list = new List<ResthookViewModel>(source.Resthooks.Count());

            foreach (ResthooksResponseDto.ResthookDto resthookDto in source.Resthooks)
            {
                list.Add(new ResthookViewModel()
                {
                    ResthookId = resthookDto.ResthookId,
                    EventName = resthookDto.EventName,
                    Status = resthookDto.Status,
                    TargetUrl = resthookDto.TargetUrl,
                    TriggeredCount = resthookDto.TriggeredCount,
                    LastTriggered = resthookDto.LastTriggered,
                });
            }

            return list;
        }


        public static List<DataFieldViewModel> ConvertToDomainModel(DataCollectionFieldsResponseDto source)
        {
            if (source == null) return new List<DataFieldViewModel>();
            if (source.DataCollectionFields.Count() == 0) return new List<DataFieldViewModel>();

            var list = new List<DataFieldViewModel>(source.DataCollectionFields.Count());

            foreach (DataCollectionFieldsResponseDto.DataCollectionFieldDto dataCollectionFieldDto in source.DataCollectionFields)
            {
                list.Add(new DataFieldViewModel()
                {
                    DataCollectionFieldId = dataCollectionFieldDto.DataCollectionFieldId,
                    DataType = TranslateInternalIdentifierToDisplayName(dataCollectionFieldDto.DataType),
                    Description = dataCollectionFieldDto.Description,
                    Label = dataCollectionFieldDto.Label
                });
            }

            return list;
        }
        

        private static string TranslateInternalIdentifierToDisplayName(string internalIdentifier)
        {
            if (internalIdentifier.Equals("Str255")) return "String";
            if (internalIdentifier.Equals("DateNM")) return "Date";
            return internalIdentifier;
        }


        public static List<DataCollectionViewModel> ConvertToDomainModel(DataCollectionsResponseDto source)
        {
            if (source == null) return new List<DataCollectionViewModel>();
            if (source.DataCollections.Count() == 0) return new List<DataCollectionViewModel>();

            var list = new List<DataCollectionViewModel>(source.DataCollections.Count());

            foreach (DataCollectionsResponseDto.DataCollectionDto dataCollectionDto in source.DataCollections)
            {
                list.Add(new DataCollectionViewModel()
                {
                    DataCollectionId = dataCollectionDto.DataCollectionId,
                    MatterType = source.LinkedMatterTypes.Single(x => x.MatterTypeId == dataCollectionDto.Links.MatterTypeId).Name,
                    Description = dataCollectionDto.Description,
                    Label = dataCollectionDto.Label
                });
            }

            return list;
        }


        public static IEnumerable<DataCollectionLookup> ConvertToDomainModelForLookup(DataCollectionsResponseDto source)
        {
            if (source.DataCollections.Count == 0) return new List<DataCollectionLookup>();
            var list = new List<DataCollectionLookup>(source.DataCollections.Count);

            foreach (var dto in source.DataCollections)
            {
                list.Add(new DataCollectionLookup()
                {
                    DataCollectionId = dto.DataCollectionId,
                    MatterTypeId = dto.Links.MatterTypeId,
                    Label = dto.Label
                });
            }

            return list;
        }


        public static List<MatterViewModel> ConvertToDomainModel(MattersResponseDto source)
        {
            if (source == null) return new List<MatterViewModel>();
            if (source.Matters.Count() == 0) return new List<MatterViewModel>();

            var list = new List<MatterViewModel>(source.Matters.Count());

            foreach(MattersResponseDto.MatterDto matterDto in source.Matters)
            {
                list.Add(new MatterViewModel()
                {
                    MatterId = matterDto.MatterId,
                    Name = matterDto.Name,
                    Status = matterDto.Status,
                    MatterType = source.LinkedMatterTypes.Single(x => x.MatterTypeId == matterDto.Links.MatterTypeId).Name,
                    MatterTypeId = matterDto.Links.MatterTypeId,
                    LastModified = matterDto.LastModified,
                    AssignedTo = source.LinkedParticipants.Single(x => x.ParticipantId == matterDto.Links.ParticipantId).DisplayName
                });
            }

            return list;
        }


        public static List<FilenoteViewModel> ConvertToDomainModel (FilenotesResponseDto source)
        {
            if (source == null) return new List<FilenoteViewModel>();
            if (source.Filenotes.Count() == 0) return new List<FilenoteViewModel>();

            var list = new List<FilenoteViewModel>(source.Filenotes.Count());

            foreach (FilenotesResponseDto.FilenoteDto filenoteDto in source.Filenotes)
            {
                list.Add(new FilenoteViewModel()
                {
                    FilenoteId = filenoteDto.FilenoteId,
                    Content = filenoteDto.Content,
                    MatterId = filenoteDto.Links.MatterId.Value,
                    MatterName = source.LinkedActions.Single(x => x.ActionId == filenoteDto.Links.MatterId.Value).Name,
                    Created = filenoteDto.Created,
                    CreatedBy = filenoteDto.CreatedBy
                });                
            }

            return list;
        }


        public static List<DocumentViewModel> ConvertToDomainModel(DocumentsResponseDto source)
        {
            if (source == null) return new List<DocumentViewModel>();
            if (source.Documents.Count() == 0) return new List<DocumentViewModel>();

            var list = new List<DocumentViewModel>(source.Documents.Count());

            foreach (DocumentsResponseDto.DocumentDto documentDto in source.Documents)
            {
                list.Add(new DocumentViewModel()
                {
                    DocumentId = documentDto.DocumentId,
                    Name = documentDto.Name,
                    Extension = documentDto.Extension,
                    FileName = documentDto.FileName,
                    FileSize = documentDto.FileSize,           
                    InternalFileIdentifier = documentDto.InternalFileIdentifier,
                    MatterId = documentDto.Links.MatterId.Value,
                    MatterName = source.LinkedActions.Single(x => x.ActionId == documentDto.Links.MatterId.Value).Name,
                    Created = documentDto.Created,
                    CreatedBy = source.LinkedParticipants.Single(x => x.ParticipantId == documentDto.Links.ParticipantId.Value).DisplayName
                });
            }

            return list;
        }


        public static IEnumerable<MatterLookup> ConvertToDomainModel(IEnumerable<MatterLookupResponseDto> source)
        {
            if (source == null) return new List<MatterLookup>();
            var list = new List<MatterLookup>(source.Count());

            foreach (var dto in source)
            {
                list.Add(new MatterLookup() 
                {
                    MatterId = dto.Id,
                    Name = dto.Name,
                });
            }

            return list;
        }


        public static IEnumerable<MatterTypeLookup> ConvertToDomainModel(IEnumerable<MatterTypeLookupResponseDto> source)
        {
            if (source == null) return new List<MatterTypeLookup>();
            var list = new List<MatterTypeLookup>(source.Count());

            foreach (var dto in source)
            {
                list.Add(new MatterTypeLookup()
                {
                    MatterTypeId = dto.Id,
                    Name = dto.Name,
                });
            }

            return list;
        }
    }
}