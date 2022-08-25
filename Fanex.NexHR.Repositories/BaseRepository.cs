namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections;
    using System.Threading.Tasks;
    using Data;
    using Data.Repository;
    using Interfaces;
    using Models;

    public class BaseRepository : IBaseRepository
    {
        public async Task<CommandResult> ExecuteNonQueryAsync(INonQueryCommand command)
        {
            using (var db = ObjectDbFactory.CreateInstance(command.GetSettingKey()))
            {
                await db.ExecuteNonQueryAsync(command);
                CommandResult commandResult = GetCommandResult(db.GetOutputParams());

                return commandResult;
            }
        }

        public async Task<CommandResult> ExecuteNonQueryAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                await db.ExecuteNonQueryAsync(criteria);
                CommandResult commandResult = GetCommandResult(db.GetOutputParams());

                return commandResult;
            }
        }

        public CommandResult ExecuteNonQuery(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                db.ExecuteNonQuery(criteria);
                CommandResult commandResult = GetCommandResult(db.GetOutputParams());

                return commandResult;
            }
        }

        private static CommandResult GetCommandResult(Hashtable hash)
        {
            var errCode = hash["errCode"];
            var errMsg = hash["errMsg"];

            return new CommandResult
            {
                Code = (errCode is DBNull) ? 0 : Convert.ToInt32(errCode),
                Message = (errMsg is DBNull) ? string.Empty : Convert.ToString(errMsg)
            };
        }
    }
}