using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Actionstep.API.WebClient.Data_Transfer_Objects.Responses;
using Actionstep.API.WebClient.Domain_Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult IndexAsync([FromBody] ResthookResponseDto responseData) // ResthookResponseDto responseData)
        {
            _appState.FilenoteResthookReceived();
            

            //string json = new StreamReader(HttpContext.Request.Body).ReadToEnd(); //.ReadToEndAsync();


            return Ok();
        }


        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello World");
        }


    }
}