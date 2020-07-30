using System;
using System.Collections.Generic;
using System.Text;
using Runbook.Models;

namespace Runbook.Services.Interfaces
{
    public interface IStageService
    {
        bool CreateStage(Stage stage, int bookId);

        IEnumerable<Stage> GetAllStages(int bookId, int envId);

        bool UpdateStageStatus(int id, int nextStageId, int statusId);
    }
}
