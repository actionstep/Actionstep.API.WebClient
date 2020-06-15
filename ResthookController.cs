using Actionstep.API.WebClient.Data_Transfer_Objects.Responses;
using Actionstep.API.WebClient.Domain_Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Actionstep.API.WebClient
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResthookController : ControllerBase
    {
        private readonly AppState _appState;

        public ResthookController(AppState appState)
        {
            _appState = appState;
        }


        [HttpPost]
        public IActionResult Index([FromBody] ResthookResponseDto responseData)
        {
            var filenoteResthookData = new FilenoteResthookResponseData()
            {
                ResthookType = responseData.data.type,
                FilenoteId = Convert.ToInt32(responseData.data.id),
                Created = responseData.data.attributes.enteredTimestamp,
                CreatedBy = responseData.data.attributes.enteredBy,
                Content = responseData.data.attributes.text
            };
            
            _appState.FilenoteResthookReceived(filenoteResthookData);

            return Ok();
        }
    }
}