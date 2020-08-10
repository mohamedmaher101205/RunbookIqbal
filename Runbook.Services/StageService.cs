using Dapper;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Runbook.Services
{

    /// <summary>
    /// This StageService class have methods to performing create a stage
    /// get all stage,modify stage
    /// </summary>
    public class StageService : IStageService
    {
        private readonly IDbConnection _Idbconnection;

        /// <summary>
        /// This constructor is to inject IDBConnection using constructor dependency injuction
        /// </summary>
        /// <param name="dbConnection"></param>
        public StageService(IDbConnection dbConnection)
        {
            _Idbconnection = dbConnection;
        }

        /// <summary>
        /// create a stage
        /// </summary>
        /// <param name="stage"></param>
        /// <param name="bookId"></param>
        /// <returns>true or false</returns>
        public bool CreateStage(Stage stage, int bookId)
        {
            try
            {

                string stagecmd = @"INSERT INTO [dbo].[STAGES](StageName,BookId,Description,EnvId,StatusId) 
                            VALUES(@StageName,@BookId,@Description,@EnvId,@StatusId)";

                string getallenvcmd = @"SELECT bookenv.BookId,env.EnvID FROM [dbo].[BookEnvironment] bookenv
                        JOIN [dbo].[userdefinedenvironments] env on bookenv.envId = env.envId
                        WHERE bookenv.bookId = @BookId";

                int stageCreated = 0;

                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();

                    IEnumerable<Environments> envs = con.Query<Environments>(getallenvcmd, new { BookId = bookId });

                    List<Stage> createStagesForEnvs = new List<Stage>();
                    foreach (var environment in envs)
                    {
                        createStagesForEnvs.Add(new Stage
                        {
                            StageName = stage.StageName,
                            Description = stage.Description,
                            BookId = bookId,
                            EnvId = environment.EnvId,
                            StatusId = 0
                        });
                    }
                    //var sqltrans = con.BeginTransaction();
                    stageCreated = con.Execute(stagecmd, createStagesForEnvs);

                    // if(stageCreated > 0){
                    //     sqltrans.Commit();
                    // }
                    // else{
                    //     sqltrans.Rollback();
                    // }
                    con.Close();
                }
                if (stageCreated > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Read all stages
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="envId"></param>
        /// <returns>list of stages</returns>
        public IEnumerable<Stage> GetAllStages(int bookId, int envId)
        {
            try
            {
                string stagescmd = @"SELECT * FROM [dbo].[STAGES] WHERE BookId=@BookId AND EnvId = @EnvId";

                IEnumerable<Stage> stages = null;
                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    stages = con.Query<Stage>(stagescmd, new { BookId = bookId, EnvId = envId });
                    con.Close();
                }
                return stages;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// modify the stage status
        /// </summary>
        /// <param name="stageid"></param>
        /// <param name="nextStageId"></param>
        /// <param name="statusId"></param>
        /// <returns>true or false</returns>
        public bool UpdateStageStatus(int stageid, int nextStageId, int statusId)
        {
            try
            {
                string stageupdatecmd = @"UPDATE [dbo].[Stages] SET StatusId = @StatusId WHERE StageId = @ID";
                string nextstagestatus = @"UPDATE [dbo].[Stages] SET StatusId = 1 WHERE StageId = @NextId";
                int stagestatusupdate = 0, nextStageStatusUpdate = 0;
                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    var sqltrans = con.BeginTransaction();
                    stagestatusupdate = con.Execute(stageupdatecmd, new { ID = stageid, StatusId = statusId }, sqltrans);
                    if (nextStageId > 0)
                    {
                        nextStageStatusUpdate = con.Execute(nextstagestatus, new { NextId = nextStageId }, sqltrans);
                    }
                    if (stagestatusupdate > 0)
                    {
                        sqltrans.Commit();
                    }
                    else
                    {
                        sqltrans.Rollback();
                    }
                    con.Close();
                }
                if (stagestatusupdate > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
