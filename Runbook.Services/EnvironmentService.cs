using Dapper;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Runbook.Services
{
    /// <summary>
    /// This EnvironmentController class have methods to performing create an environment and 
    /// select all environments
    /// </summary>
    public class EnvironmentService : IEnvironmentService
    {
        private readonly IDbConnection _Idbconnection;

        /// <summary>
        /// This constructor is to inject IDBConnection using constructor dependency injuction
        /// </summary>
        /// <param name="dbConnection"></param>
        public EnvironmentService(IDbConnection dbConnection)
        {
            _Idbconnection = dbConnection;
        }

        /// <summary>
        /// create an environment
        /// </summary>
        /// <param name="env"></param>
        /// <param name="tenantId"></param>
        /// <returns>Success message</returns>
        public int CreateEnvironment(Environments env, int tenantId)
        {
            try
            {
                string userDefinedEnv = @"INSERT INTO [dbo].[UserDefinedEnvironments](Environment,TenantId)
                                            VALUES(@Environment,@TenantId)";
                int insertedEnv = 0;
                using (IDbConnection con = _Idbconnection)
                {
                    con.Open();
                    var sqlTrans = con.BeginTransaction();
                    insertedEnv = con.Execute(userDefinedEnv, new
                    {
                        Environment = env.Environment,
                        TenantId = tenantId
                    }, sqlTrans);
                    if (insertedEnv > 0)
                    {
                        sqlTrans.Commit();
                    }
                    else
                    {
                        sqlTrans.Rollback();
                    }
                    con.Close();
                }
                return insertedEnv;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// select all environment list
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of environment</returns>
        public IEnumerable<Environments> GetAllEnvironments(int tenantId)
        {
            string envcmd = @"SELECT * FROM [dbo].[userdefinedenvironments] WHERE TenantId = @TenantId";

            IEnumerable<Environments> envs = null;

            using (IDbConnection con = _Idbconnection)
            {
                con.Open();
                envs = con.Query<Environments>(envcmd, new { TenantId = tenantId });
                con.Close();
            }
            return envs;
        }
    }
}
