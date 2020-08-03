using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace Runbook.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class StageController : ControllerBase
    {
        private readonly IStageService _stage;
        private readonly ILogger _logger;

        public StageController(ILogger<StageController> logger, IStageService stage)
        {
            _logger = logger;
            _stage = stage;
        }

        [HttpPost]
        [Route("CreateStage/{bookId}")]
        public IActionResult CreateStage(Stage stage, int bookId)
        {
            try
            {
                if (bookId > 0 && !string.IsNullOrEmpty(stage.StageName))
                {

                    bool res = _stage.CreateStage(stage, bookId);

                    if (res)
                    {
                        return Ok("Stage created successfully");
                    }
                    else
                    {
                        return Ok("Unsuccessful while creating stage");
                    }
                }
                else
                {
                    _logger.LogError($"Invalid BookID : {bookId} or Stage : {stage} in CreateStage");
                    return BadRequest("Invalid BookId or Stage Name");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong, Internal server error : {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        [Route("GetStages/{BookId}/{EnvId}")]
        public ActionResult<IEnumerable<Stage>> GetAllStages(int bookId, int envId)
        {
            try
            {
                if (bookId > 0 && envId > 0)
                {
                    return Ok(_stage.GetAllStages(bookId, envId));
                }
                else
                {
                    _logger.LogError($"Invalid Book Id : {bookId} or Environment Id : {envId} in GetAllStages");
                    return BadRequest($"Invalid Book Id : {bookId} or Environment Id : {envId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server error : {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut]
        [Route("UpdateStage/{stageId}/{nextStageId}/{statusId}")]
        public ActionResult<bool> UpdateStageStatus(int stageId, int nextStageId, int statusId)
        {
            try
            {
                if (stageId > 0 && nextStageId >= 0 && statusId >= 0)
                {
                    return Ok(_stage.UpdateStageStatus(stageId, nextStageId, statusId));
                }
                else
                {
                    _logger.LogError($"Invalid stageId : {stageId} or nextStageId : {nextStageId} or StatusId : {statusId} in UpdateStageStatus");
                    return BadRequest($"Invalid stageId : {stageId} or nextStageId : {nextStageId} or StatusId : {statusId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal Server Error in UpdateStageStatus : {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}