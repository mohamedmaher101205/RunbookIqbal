using Dapper;
using Runbook.Models;
using Runbook.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Runbook.Services
{
    public class EnvironmentService : IEnvironmentService
    {
        private readonly IDbConnection _Idbconnection;

        public EnvironmentService(IDbConnection dbConnection)
        {
            _Idbconnection = dbConnection;
        }

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
