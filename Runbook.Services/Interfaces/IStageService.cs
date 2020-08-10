using System;
using System.Collections.Generic;
using System.Text;
using Runbook.Models;

namespace Runbook.Services.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IStageService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stage"></param>
        /// <param name="bookId"></param>
        /// <returns></returns>
        bool CreateStage(Stage stage, int bookId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="envId"></param>
        /// <returns></returns>
        IEnumerable<Stage> GetAllStages(int bookId, int envId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nextStageId"></param>
        /// <param name="statusId"></param>
        /// <returns></returns>
        bool UpdateStageStatus(int id, int nextStageId, int statusId);
    }
}
