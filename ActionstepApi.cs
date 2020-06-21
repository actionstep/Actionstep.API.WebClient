using Actionstep.API.WebClient.Data_Transfer_Objects.Requests;
using Actionstep.API.WebClient.Data_Transfer_Objects.Responses;
using Actionstep.API.WebClient.Domain_Models;
using BlazorInputFile;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace Actionstep.API.WebClient
{
    public class ActionstepApi
    {
        private const int MAX_PAGE_SIZE = 50;

        private readonly HttpClient _httpClient;
        private readonly AppConfiguration _appConfiguration;

        private AppState _appState;
        private AsyncRetryPolicy<HttpResponseMessage> _policy;

        private IDictionary<string, object> ContextData => new Dictionary<string, object> { };


        public ActionstepApi(HttpClient httpClient, AppConfiguration appConfiguration, AppState appState)
        {
            _httpClient = httpClient;
            _appConfiguration = appConfiguration;
            _appState = appState;

            _httpClient.BaseAddress = new Uri(_appState.ApiEndpoint);

            _policy = Policy.HandleResult<HttpResponseMessage>(message => message.StatusCode == HttpStatusCode.Unauthorized)
                            .RetryAsync(1, async (result, retryCount, context) =>
                            {
                                var accessTokenDto = await RefreshAccessTokenAsync();
                                if (accessTokenDto != null)
                                    UpdateAppState(accessTokenDto);
                            });
        }

        #region Matters (Actions)
        public async Task<MattersResponseDto> GetMattersAsync(int pageNumber, int pageSize, int filterByMatterTypeId)
        {
            var mattersDto = new MattersResponseDto();

            var matterTypeFilter = filterByMatterTypeId == -1 ? String.Empty : $"&actionType_eq={filterByMatterTypeId}";

            var response = await _policy.ExecuteAsync(async context =>
            {                
                var message = new HttpRequestMessage(HttpMethod.Get, $"/api/rest/actions?include=actionType,assignedTo&fields[actions]=id,name,status,modifiedTimestamp,actionType,assignedTo&fields[actiontypes]=id,name&fields[participants]=id,displayName&page={pageNumber}&pageSize={pageSize}&sort=-modifiedTimestamp{matterTypeFilter}");
                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!String.IsNullOrEmpty(responseContent))
                {
                    var data = JObject.Parse(responseContent);

                    mattersDto.PageMetaDataDto = data["meta"]["paging"]["actions"].ToObject<PageMetaDataResponseDto>();

                    if (mattersDto.PageMetaDataDto.DataRows == 1)
                    {
                        mattersDto.Matters.Add(data["actions"].ToObject<MattersResponseDto.MatterDto>());
                    }
                    else
                    {
                        foreach (JToken item in data["actions"].Children())
                        {
                            mattersDto.Matters.Add(item.ToObject<MattersResponseDto.MatterDto>());
                        }
                    }

                    foreach (JToken item in data["linked"]["actiontypes"].Children())
                    {
                        mattersDto.LinkedMatterTypes.Add(item.ToObject<MattersResponseDto.LinkedMatterTypeDto>());
                    }

                    foreach (JToken item in data["linked"]["participants"].Children())
                    {
                        mattersDto.LinkedParticipants.Add(item.ToObject<MattersResponseDto.LinkedParticipantDto>());
                    }
                }
            }
            return mattersDto;
        }


        public async Task<IEnumerable<MatterLookupResponseDto>> GetMatterLookupAsync()
        {
            var matters = new List<MatterLookupResponseDto>();

            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Get, "/api/rest/actions?fields=id,name&status_nteq=Closed");
                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!String.IsNullOrEmpty(responseContent))
                {
                    var data = JObject.Parse(responseContent);
                    foreach (JToken item in data["actions"])
                    {
                        matters.Add(item.ToObject<MatterLookupResponseDto>());
                    }
                }
            }
            return matters;
        }
        #endregion

        #region Matter Types
        public async Task<IEnumerable<MatterTypeLookupResponseDto>> GetMatterTypesLookupAsync()
        {
            var actions = new List<MatterTypeLookupResponseDto>();

            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Get, "/api/rest/actiontypes?fields=id,name");
                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!String.IsNullOrEmpty(responseContent))
                {
                    var data = JObject.Parse(responseContent);
                    foreach (JToken item in data["actiontypes"])
                    {
                        actions.Add(item.ToObject<MatterTypeLookupResponseDto>());
                    }
                }
            }
            return actions;
        }
        #endregion

        #region Data Collections and Data Collection Fields
        public async Task<DataCollectionsResponseDto.DataCollectionDto> GetDataCollectionAsync(int dataCollectionId)
        {
            var dataCollectionDto = new DataCollectionsResponseDto.DataCollectionDto();

            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Get, $"/api/rest/datacollections/{dataCollectionId}?include=actionType");
                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!String.IsNullOrEmpty(responseContent))
                {
                    var data = JObject.Parse(responseContent);
                    dataCollectionDto = data["datacollections"].ToObject<DataCollectionsResponseDto.DataCollectionDto>();
                }
            }
            return dataCollectionDto;
        }


        public async Task<DataCollectionsResponseDto> GetDataCollectionsAsync(int actionTypeId = -1)
        {
            return await GetDataCollectionsAsync(1, MAX_PAGE_SIZE, actionTypeId);
        }


        public async Task<DataCollectionsResponseDto> GetDataCollectionsAsync(int pageNumber, int pageSize, int actionTypeId = -1)
        {
            var dataCollectionsDto = new DataCollectionsResponseDto();

            var actionTypeFilter = actionTypeId == -1 ? String.Empty : $"&actionType={actionTypeId}";

            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Get, $"/api/rest/datacollections?include=actionType&multipleRecords=F{actionTypeFilter}&page={pageNumber}&pageSize={pageSize}");
                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!String.IsNullOrEmpty(responseContent))
                {
                    var data = JObject.Parse(responseContent);

                    dataCollectionsDto.PageMetaDataDto = data["meta"]["paging"]["datacollections"].ToObject<PageMetaDataResponseDto>();

                    if (dataCollectionsDto.PageMetaDataDto.DataRows == 1)
                    {
                        dataCollectionsDto.DataCollections.Add(data["datacollections"].ToObject<DataCollectionsResponseDto.DataCollectionDto>());
                    }
                    else
                    {
                        foreach (JToken item in data["datacollections"].Children())
                        {
                            dataCollectionsDto.DataCollections.Add(item.ToObject<DataCollectionsResponseDto.DataCollectionDto>());
                        }
                    }

                    foreach (JToken item in data["linked"]["actiontypes"].Children())
                    {
                        dataCollectionsDto.LinkedMatterTypes.Add(item.ToObject<DataCollectionsResponseDto.LinkedMatterTypeDto>());
                    }
                }
            }
            return dataCollectionsDto;
        }


        public async Task<DataCollectionFieldsResponseDto.DataCollectionFieldDto> GetDataFieldAsync(string dataFieldId)
        {
            var dataFieldDto = new DataCollectionFieldsResponseDto.DataCollectionFieldDto();

            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Get, $"/api/rest/datacollectionfields/{dataFieldId}");
                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!String.IsNullOrEmpty(responseContent))
                {
                    var data = JObject.Parse(responseContent);
                    dataFieldDto = data["datacollectionfields"].ToObject<DataCollectionFieldsResponseDto.DataCollectionFieldDto>();
                }
            }
            return dataFieldDto;
        }


        public async Task<DataCollectionFieldsResponseDto> GetDataCollectionFieldsAsync(int dataCollectionId)
        {
            return await GetDataCollectionFieldsAsync(1, MAX_PAGE_SIZE, dataCollectionId);
        }


        public async Task<DataCollectionFieldsResponseDto> GetDataCollectionFieldsAsync(int pageNumber, int pageSize, int dataCollectionId)
        {
            var dataCollectionFields = new DataCollectionFieldsResponseDto();

            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Get, $"/api/rest/datacollectionfields?dataCollection={dataCollectionId}&page={pageNumber}&pageSize={pageSize}");
                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!String.IsNullOrEmpty(responseContent))
                {
                    var data = JObject.Parse(responseContent);

                    dataCollectionFields.PageMetaDataDto = data["meta"]["paging"]["datacollectionfields"].ToObject<PageMetaDataResponseDto>();

                    if (dataCollectionFields.PageMetaDataDto.DataRows == 1)
                    {
                        dataCollectionFields.DataCollectionFields.Add(data["datacollectionfields"].ToObject<DataCollectionFieldsResponseDto.DataCollectionFieldDto>());
                    }
                    else
                    {
                        foreach (JToken item in data["datacollectionfields"].Children())
                        {
                            dataCollectionFields.DataCollectionFields.Add(item.ToObject<DataCollectionFieldsResponseDto.DataCollectionFieldDto>());
                        }
                    }
                }
            }
            return dataCollectionFields;
        }


        public async Task<DataCollectionRecordValuesResponseDto> GetDataCollectionRecordValuesAsync(IEnumerable<int> matterIds, int matterTypeId)
        {
            var dataCollectionRecordValues = new DataCollectionRecordValuesResponseDto();

            string matterList = string.Join(",", matterIds);

            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Get, $"/api/rest/datacollectionrecordvalues?action={matterList}&include=dataCollectionField,action,dataCollection&fields=dataCollectionField&fields[datacollectionfields]=name,datatype,label,formOrder,description&action[actionType]={matterTypeId}&dataCollection[multipleRecords_eq]=F");
                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!String.IsNullOrEmpty(responseContent))
                {
                    var data = JObject.Parse(responseContent);

                    dataCollectionRecordValues.PageMetaDataDto = data["meta"]["paging"]["datacollectionrecordvalues"].ToObject<PageMetaDataResponseDto>();

                    if (dataCollectionRecordValues.PageMetaDataDto.DataRows == 1)
                    {
                        dataCollectionRecordValues.DataCollectionRecordValues.Add(data["datacollectionrecordvalues"].ToObject<DataCollectionRecordValuesResponseDto.DataCollectionRecordValueDto>());
                    }
                    else
                    {
                        foreach (JToken item in data["datacollectionrecordvalues"].Children())
                        {
                            dataCollectionRecordValues.DataCollectionRecordValues.Add(item.ToObject<DataCollectionRecordValuesResponseDto.DataCollectionRecordValueDto>());
                        }
                    }

                    foreach (JToken item in data["linked"]["datacollectionfields"].Children())
                    {
                        dataCollectionRecordValues.LinkedDataCollectionFields.Add(item.ToObject<DataCollectionRecordValuesResponseDto.LinkedDataCollectionFieldDto>());
                    }
                }
            }

            return dataCollectionRecordValues;
        }


        public async Task<bool> CreateDataCollectionAsync(int matterTypeId, string name, string description, string label)
        {
            var dto = new CreateDataCollectionRequestDto();
            dto.DataCollections.Name = name;
            dto.DataCollections.Label = label;
            dto.DataCollections.Description = description;

            dto.DataCollections.DataCollectionLinks = new CreateDataCollectionRequestDto.DataCollection.Links
            {
                MatterTypeId = matterTypeId
            };

            var jsonDto = JsonConvert.SerializeObject(dto);
            var content = new StringContent(jsonDto);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.api+json");

            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Post, $"/api/rest/datacollections")
                {
                    Content = content
                };

                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            return response.IsSuccessStatusCode;
        }


        public async Task<bool> UpdateDataCollectionAsync(int dataCollectionId, int matterTypeId, string name, string description, string label)
        {
            var dto = new UpdateDataCollectionRequestDto();
            dto.DataCollections.Name = name;
            dto.DataCollections.Label = label;
            dto.DataCollections.Description = description;

            dto.DataCollections.DataCollectionLinks = new UpdateDataCollectionRequestDto.DataCollection.Links
            {
                MatterTypeId = matterTypeId
            };

            var jsonDto = JsonConvert.SerializeObject(dto);
            var content = new StringContent(jsonDto);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.api+json");

            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Put, $"/api/rest/datacollections/{dataCollectionId}")
                {
                    Content = content
                };

                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            return response.IsSuccessStatusCode;
        }


        public async Task<bool> DeleteDataCollectionAsync(int dataCollectionId)
        {
            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Delete, $"/api/rest/datacollections/{dataCollectionId}");
                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            return response.IsSuccessStatusCode;
        }


        public async Task<bool> CreateDataFieldAsync(int dataCollectionId, string name, string description, string label, string dataType)
        {
            var dto = new CreateDataFieldRequestDto();
            dto.DataFields.Name = name;
            dto.DataFields.Label = label;
            dto.DataFields.Description = description;
            dto.DataFields.DataType = dataType;

            dto.DataFields.DataFieldLinks = new CreateDataFieldRequestDto.DataField.Links
            {
                DataCollectionId = dataCollectionId
            };

            var jsonDto = JsonConvert.SerializeObject(dto);
            var content = new StringContent(jsonDto);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.api+json");

            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Post, $"/api/rest/datacollectionfields")
                {
                    Content = content
                };

                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            return response.IsSuccessStatusCode;
        }


        public async Task<bool> UpdateDataFieldAsync(string dataFieldId, string name, string description, string label)
        {
            var dto = new UpdateDataFieldRequestDto();
            dto.DataFields.Name = name;
            dto.DataFields.Label = label;
            dto.DataFields.Description = description;

            var jsonDto = JsonConvert.SerializeObject(dto);
            var content = new StringContent(jsonDto);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.api+json");

            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Put, $"/api/rest/datacollectionfields/{dataFieldId}")
                {
                    Content = content
                };

                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            return response.IsSuccessStatusCode;
        }


        public async Task<bool> DeleteDataFieldAsync(string dataFieldId)
        {
            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Delete, $"/api/rest/datacollectionfields/{dataFieldId}");
                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            return response.IsSuccessStatusCode;
        }


        public async Task<bool> UpdateDataFieldValue(string dataCollectionRecordValueId, string dataFieldValue)
        {
            var dto = new UpdateDataFieldValueRequestDto();
            dto.DataFields.DataValue = dataFieldValue;

            var jsonDto = JsonConvert.SerializeObject(dto);
            var content = new StringContent(jsonDto);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.api+json");

            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Put, $"/api/rest/datacollectionrecordvalues/{dataCollectionRecordValueId}")
                {
                    Content = content
                };

                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            return response.IsSuccessStatusCode;
        }
        #endregion

        #region File notes
        public async Task<FilenotesResponseDto.FilenoteDto> GetFilenoteAsync(int filenoteId)
        {
            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Get, $"/api/rest/filenotes/{filenoteId}");
                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!String.IsNullOrEmpty(responseContent))
                {
                    var data = JObject.Parse(responseContent);
                    return data["filenotes"].ToObject<FilenotesResponseDto.FilenoteDto>();
                }
            }
            return null;
        }


        public async Task<FilenotesResponseDto> GetFilenotesAsync(int pageNumber, int pageSize, int filterByMatterId)
        {
            var filenotesDto = new FilenotesResponseDto();

            var matterFilter = filterByMatterId == -1 ? String.Empty : $"&action_eq={filterByMatterId}";

            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Get, $"/api/rest/filenotes?include=action&source_nteq=System{matterFilter}&page={pageNumber}&pageSize={pageSize}&sort=-enteredTimestamp&fields[filenotes]=id,enteredTimestamp,text,enteredBy,action&fields[actions]=id,name");
                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!String.IsNullOrEmpty(responseContent))
                {
                    var data = JObject.Parse(responseContent);

                    filenotesDto.PageMetaDataDto = data["meta"]["paging"]["filenotes"].ToObject<PageMetaDataResponseDto>();

                    if (filenotesDto.PageMetaDataDto.DataRows == 1)
                    {
                        filenotesDto.Filenotes.Add(data["filenotes"].ToObject<FilenotesResponseDto.FilenoteDto>());
                    }
                    else
                    {
                        foreach (JToken item in data["filenotes"].Children())
                        {
                            filenotesDto.Filenotes.Add(item.ToObject<FilenotesResponseDto.FilenoteDto>());
                        }
                    }

                    foreach (JToken item in data["linked"]["actions"].Children())
                    {
                        filenotesDto.LinkedActions.Add(item.ToObject<FilenotesResponseDto.LinkedActionDto>());
                    }
                }
            }
            return filenotesDto;
        }


        public async Task<FilenotesResponseDto> SearchFilenotesAsync(string searchString, int pageSize)
        {
            var filenotesDto = new FilenotesResponseDto();

            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Get, $"/api/rest/filenotes?include=action&source_nteq=System&text_ilike=*{searchString}*&page=1&pageSize={pageSize}&sort=-enteredTimestamp&fields[filenotes]=id,enteredTimestamp,text,enteredBy,action&fields[actions]=id,name");
                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            if (response.IsSuccessStatusCode)
            {
                var searchResults = await response.Content.ReadAsStringAsync();
                if (!String.IsNullOrEmpty(searchResults))
                { 
                    var data = JObject.Parse(searchResults);

                    filenotesDto.PageMetaDataDto = data["meta"]["paging"]["filenotes"].ToObject<PageMetaDataResponseDto>();

                    if (filenotesDto.PageMetaDataDto.DataRows == 1)
                    {
                        filenotesDto.Filenotes.Add(data["filenotes"].ToObject<FilenotesResponseDto.FilenoteDto>());
                    }
                    else
                    {
                        foreach (JToken item in data["filenotes"].Children())
                        {
                            filenotesDto.Filenotes.Add(item.ToObject<FilenotesResponseDto.FilenoteDto>());
                        }
                    }

                    foreach (JToken item in data["linked"]["actions"].Children())
                    {
                        filenotesDto.LinkedActions.Add(item.ToObject<FilenotesResponseDto.LinkedActionDto>());
                    }
                }
            }
            return filenotesDto;
        }


        public async Task<bool> CreateFilenoteAsync(int matterId, string noteContent)
        {
            var dto = new CreateFilenoteRequestDto();
            dto.Filenotes.Content = noteContent;
            dto.Filenotes.FilenoteLinks = new CreateFilenoteRequestDto.Filenote.Links
            {
                MatterId = matterId,
                ContactId = await GetSignedInUserId()
            };

            var jsonDto = JsonConvert.SerializeObject(dto);
            var content = new StringContent(jsonDto);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.api+json");

            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Post, $"/api/rest/filenotes")
                {
                    Content = content
                };

                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            return response.IsSuccessStatusCode;
        }


        public async Task<bool> UpdateFilenoteAsync(int filenoteId, string noteContent)
        {
            var dto = new UpdateFilenoteRequestDto();
            dto.Filenotes.Content = noteContent;

            var jsonDto = JsonConvert.SerializeObject(dto);
            var content = new StringContent(jsonDto);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.api+json");

            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Put, $"/api/rest/filenotes/{filenoteId}")
                {
                    Content = content
                };

                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            return response.IsSuccessStatusCode;
        }


        public async Task<bool> DeleteFilenoteAsync(int filenoteId)
        {
            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Delete, $"/api/rest/filenotes/{filenoteId}");
                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            return response.IsSuccessStatusCode;
        }


        public async Task<bool> DeleteMultipleFilenotesAsync(IEnumerable<int> filenotesToDelete)
        {
            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Delete, $"/api/rest/filenotes/{string.Join(",", filenotesToDelete)}");
                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            return response.IsSuccessStatusCode;
        }
        #endregion

        #region Matter documents
        public async Task<DocumentsResponseDto.DocumentDto> GetDocumentAsync(int documentId)
        {
            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Get, $"/api/rest/actiondocuments/{documentId}");
                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!String.IsNullOrEmpty(responseContent))
                {
                    var data = JObject.Parse(responseContent);
                    return data["actiondocuments"].ToObject<DocumentsResponseDto.DocumentDto>();
                }
            }
            return null;
        }


        public async Task<DocumentsResponseDto> GetDocumentsAsync(int pageNumber, int pageSize, int filterByMatterId = -1)
        {
            var documentsDto = new DocumentsResponseDto();

            var matterFilter = filterByMatterId == -1 ? String.Empty : $"&action_eq={filterByMatterId}";

            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Get, $"/api/rest/actiondocuments?include=action,createdBy&fields[actiondocuments]=id,name,modifiedTimestamp,fileSize,extension,fileName,file,action,createdBy&fields[actions]=id,name&fields[participants]=id,displayName&page={pageNumber}&pageSize={pageSize}&sort=-modifiedTimestamp{matterFilter}");
                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!String.IsNullOrEmpty(responseContent))
                {
                    var data = JObject.Parse(responseContent);

                    documentsDto.PageMetaDataDto = data["meta"]["paging"]["actiondocuments"].ToObject<PageMetaDataResponseDto>();

                    if (documentsDto.PageMetaDataDto.DataRows == 1)
                    {
                        documentsDto.Documents.Add(data["actiondocuments"].ToObject<DocumentsResponseDto.DocumentDto>());
                    }
                    else
                    {
                        foreach (JToken item in data["actiondocuments"].Children())
                        {
                            documentsDto.Documents.Add(item.ToObject<DocumentsResponseDto.DocumentDto>());
                        }
                    }

                    foreach (JToken item in data["linked"]["actions"].Children())
                    {
                        documentsDto.LinkedActions.Add(item.ToObject<DocumentsResponseDto.LinkedActionDto>());
                    }

                    foreach (JToken item in data["linked"]["participants"].Children())
                    {
                        documentsDto.LinkedParticipants.Add(item.ToObject<DocumentsResponseDto.LinkedParticipantDto>());
                    }
                }
            }
            return documentsDto;
        }


        public async Task<bool> CreateDocumentAsync(string name, string uploadedFileId, int parentMatterId)
        {
            var dto = new CreateDocumentRequestDto();
            dto.Documents.Name = name;
            dto.Documents.FileId = uploadedFileId;

            dto.Documents.DocumentLinks = new CreateDocumentRequestDto.Document.Links()
            { 
                MatterId = parentMatterId
            };

            var jsonDto = JsonConvert.SerializeObject(dto);
            var content = new StringContent(jsonDto);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.api+json");

            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Post, $"/api/rest/actiondocuments")
                {
                    Content = content
                };

                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            return response.IsSuccessStatusCode;
        }


        public async Task<bool> UpdateDocumentAsync(int documentId, string name, int parentMatterId)
        {
            var dto = new UpdateDocumentRequestDto();
            dto.Documents.Name = name;
            dto.Documents.DocumentLinks.MatterId = parentMatterId;

            var jsonDto = JsonConvert.SerializeObject(dto);
            var content = new StringContent(jsonDto);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.api+json");

            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Put, $"/api/rest/actiondocuments/{documentId}")
                {
                    Content = content
                };

                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            return response.IsSuccessStatusCode;
        }


        public async Task<bool> DeleteDocumentAsync(int documentId)
        {
            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Delete, $"/api/rest/actiondocuments/{documentId}");
                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            return response.IsSuccessStatusCode;
        }


        public async Task<bool> DeleteMultipleDocumentsAsync(IEnumerable<int> documentsToDelete)
        {
            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Delete, $"/api/rest/actiondocuments/{string.Join(",", documentsToDelete)}");
                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            return response.IsSuccessStatusCode;
        }
        #endregion

        #region Upload and Download files
        public async Task<string> UploadFileAsync(IFileListEntry fileToUpload)
        {
            string fileIdentifier = null;
            string boundary = Guid.NewGuid().ToString();

            int chunkSize = 5242880; // 5MB max chunk size.
            int totalChunks = (int)(fileToUpload.Size / chunkSize);
            if (fileToUpload.Size % chunkSize != 0)
            {
                totalChunks++;
            }

            for (int i = 0; i < totalChunks; i++)
            {
                long position = (i * (long)chunkSize);
                int toRead = (int)Math.Min(fileToUpload.Size - position + 1, chunkSize);
                byte[] buffer = new byte[toRead];
                await fileToUpload.Data.ReadAsync(buffer, 0, buffer.Length);

                var content = new MultipartFormDataContent();
                content.Add(new ByteArrayContent(buffer), "file", fileToUpload.Name);
                content.Headers.ContentType.MediaType = "multipart/form-data";

                var response = await _policy.ExecuteAsync(async context =>
                {
                    var message = new HttpRequestMessage(HttpMethod.Post, $"/api/rest/files?part_number={i + 1}&part_count={totalChunks}")
                    {
                        Content = content
                    };

                    message = SetMessageDefaultHeaders(message);
                    return await _httpClient.SendAsync(message);
                }, ContextData);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (!String.IsNullOrEmpty(responseContent))
                    {
                        var data = JObject.Parse(responseContent);
                        var fileDto = data["files"].ToObject<FileResponseDto>();
                        if (fileDto.Status.ToLower() == "uploaded")
                            fileIdentifier = fileDto.Id;
                    }
                }
            }
            return fileIdentifier;
        }


        public async Task<byte[]> DownloadFileAsync(string internalfileIdentifier, int fileSize)
        {
            int chunkSize = 5242880; // 5MB max chunk size.
            int totalChunks = (int)(fileSize / chunkSize);
            if (fileSize % chunkSize != 0)
            {
                totalChunks++;
            }

            using MemoryStream ms = new MemoryStream();

            for (int i = 0; i < totalChunks; i++)
            {
                var response = await _policy.ExecuteAsync(async context =>
                {
                    var message = new HttpRequestMessage(HttpMethod.Get, $"/api/rest/files/{internalfileIdentifier}?part_number={i + 1}");
                    message = SetMessageDefaultHeaders(message);
                    return await _httpClient.SendAsync(message);
                }, ContextData);

                if (response.IsSuccessStatusCode)
                {
                    var byteData = await response.Content.ReadAsByteArrayAsync();
                    ms.Write(byteData, 0, byteData.Length);
                }
            }            
            return ms.ToArray();
        }
        #endregion

        #region Web hooks 
        public async Task<ResthooksResponseDto.ResthookDto> GetResthookAsync(int resthookId)
        {
            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Get, $"/api/rest/resthooks/{resthookId}");
                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!String.IsNullOrEmpty(responseContent))
                {
                    var data = JObject.Parse(responseContent);
                    return data["resthooks"].ToObject<ResthooksResponseDto.ResthookDto>();
                }
            }
            return null;
        }


        public async Task<ResthooksResponseDto> GetResthooksAsync()
        {
            return await GetResthooksAsync(1, MAX_PAGE_SIZE);
        }


        public async Task<ResthooksResponseDto> GetResthooksAsync(int pageNumber, int pageSize)
        {
            var resthooksDto = new ResthooksResponseDto();

            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Get, $"/api/rest/resthooks?page={pageNumber}&pageSize={pageSize}");
                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!String.IsNullOrEmpty(responseContent))
                {
                    var data = JObject.Parse(responseContent);

                    resthooksDto.PageMetaDataDto = data["meta"]["paging"]["resthooks"].ToObject<PageMetaDataResponseDto>();

                    if (resthooksDto.PageMetaDataDto.DataRows == 1)
                    {
                        resthooksDto.Resthooks.Add(data["resthooks"].ToObject<ResthooksResponseDto.ResthookDto>());
                    }
                    else
                    {
                        foreach (JToken item in data["resthooks"].Children())
                        {
                            resthooksDto.Resthooks.Add(item.ToObject<ResthooksResponseDto.ResthookDto>());
                        }
                    }
                }
            }
            return resthooksDto;
        }


        public async Task<bool> CreateResthookAsync(string eventName, string targetUrl)
        {
            var dto = new CreateResthookRequestDto();
            dto.Resthooks.EventName = eventName;
            dto.Resthooks.TargetUrl = targetUrl;

            var jsonDto = JsonConvert.SerializeObject(dto);
            var content = new StringContent(jsonDto);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.api+json");

            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Post, $"/api/rest/resthooks")
                {
                    Content = content
                };

                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            return response.IsSuccessStatusCode;
        }


        public async Task<bool> UpdateResthookAsync(int resthookId, string eventName, string targetUrl)
        {
            var dto = new UpdateResthookRequestDto();
            dto.Resthooks.EventName = eventName;
            dto.Resthooks.TargetUrl = targetUrl;

            var jsonDto = JsonConvert.SerializeObject(dto);
            var content = new StringContent(jsonDto);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.api+json");

            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Put, $"/api/rest/resthooks/{resthookId}")
                {
                    Content = content
                };

                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            return response.IsSuccessStatusCode;
        }


        public async Task<bool> DeleteResthookAsync(int resthookId)
        {
            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Delete, $"/api/rest/resthooks/{resthookId}");
                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            return response.IsSuccessStatusCode;
        }


        public async Task<bool> DeleteMultipleResthooksAsync(IEnumerable<int> resthooksToDelete)
        {
            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Delete, $"/api/rest/resthooks/{string.Join(",", resthooksToDelete)}");
                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            return response.IsSuccessStatusCode;
        }
        #endregion

        #region Current User.
        public async Task<int> GetSignedInUserId()
        {
            if (_appState.UserId == 0)
            {
                var user = await GetCurrentUser();
                if (user != null)
                {
                    _appState.UserId = user.Id;
                    _appState.Firstname = user.Firstname;
                    _appState.Lastname = user.Lastname;
                }
            }
            return _appState.UserId;
        }


        private async Task<CurrentUserResponseDto> GetCurrentUser()
        {
            var response = await _policy.ExecuteAsync(async context =>
            {
                var message = new HttpRequestMessage(HttpMethod.Get, $"/api/rest/users/current?include=participant&fields[participants]=id,firstName,lastName");
                message = SetMessageDefaultHeaders(message);
                return await _httpClient.SendAsync(message);
            }, ContextData);

            if (response.IsSuccessStatusCode)
            {
                var data = JObject.Parse(await response.Content.ReadAsStringAsync());
                return data["linked"]["participants"].First.ToObject<CurrentUserResponseDto>();
            }
            return null;
        }
        #endregion

        private HttpRequestMessage SetMessageDefaultHeaders(HttpRequestMessage message)
        {
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _appState.AccessToken);
            message.Headers.TryAddWithoutValidation("Content-Type", "application/vnd.api+json");
            message.Headers.TryAddWithoutValidation("Accept", "application/vnd.api+json");
            message.Headers.Add("User-Agent", "MyApp");
            return message;
        }


        private void UpdateAppState(AccessTokenResponseDto accessTokenDto)
        {
            _appState.AccessToken = accessTokenDto.access_token;
            _appState.TokenType = accessTokenDto.token_type;
            _appState.ExpiresIn = accessTokenDto.expires_in;
            _appState.ApiEndpoint = accessTokenDto.api_endpoint;
            _appState.Orgkey = accessTokenDto.orgkey;
            _appState.RefreshToken = accessTokenDto.refresh_token;
        }


        private async Task<AccessTokenResponseDto> RefreshAccessTokenAsync()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");

            var kvCollection = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("refresh_token", _appState.RefreshToken),
                new KeyValuePair<string, string>("client_id", _appConfiguration.ClientId),
                new KeyValuePair<string, string>("client_secret", _appConfiguration.ClientSecret),
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("redirect_uri", HttpUtility.HtmlEncode(_appConfiguration.RedirectUrl))
            };

            var body = new FormUrlEncodedContent(kvCollection);

            var response = await client.PostAsync(_appConfiguration.AccessTokenUrl, body);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<AccessTokenResponseDto>(data);
            }
            return null;
        }
    }
}