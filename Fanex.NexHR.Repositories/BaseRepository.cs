namespace Fanex.NexHR.Repositories
{
    using Core.Attributes;
    using Criteria;
    using Data;
    using Data.Repository;
    using Interfaces;
    using Models;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    public class BaseRepository : IBaseRepository
    {
        public CommandResult Insert(ICriteria criteria, bool hasGetOutputParams = false)
        {
            return ExecuteNonQuery(criteria, hasGetOutputParams);
        }

        public CommandResult Update(ICriteria criteria, bool hasGetOutputParams = false)
        {
            return ExecuteNonQuery(criteria, hasGetOutputParams);
        }

        public CommandResult Delete(ICriteria criteria, bool hasGetOutputParams = false)
        {
            return ExecuteNonQuery(criteria, hasGetOutputParams);
        }

        public async Task<CommandResult> ExecuteNonQueryAsync(ICriteria criteria, bool hasGetOutputParams = false)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                Hashtable hash = criteria.GetSpParams();
                hash = (new ResultCriteria()).AddSpParams(hash);
                await db.ExecuteNonQueryAsync(hash).ConfigureAwait(false);
                if (hasGetOutputParams)
                {
                    SetValueForOutputProperty(criteria, hash);
                }

                return GetCommandResult(hash);
            }
        }

        private CommandResult ExecuteNonQuery(ICriteria criteria, bool hasGetOutputParams = false)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                Hashtable hash = criteria.GetSpParams();
                hash = (new ResultCriteria()).AddSpParams(hash);
                db.ExecuteNonQuery(hash);
                if (hasGetOutputParams)
                {
                    SetValueForOutputProperty(criteria, hash);
                }

                return GetCommandResult(hash);
            }
        }

        private CommandResult GetCommandResult(Hashtable hash)
        {
            var errCode = hash["errcode"];
            var errMsg = hash["errmsg"];

            return new CommandResult
            {
                Code = (errCode is DBNull) ? 0 : Convert.ToInt32(errCode),
                Message = (errMsg is DBNull) ? string.Empty : Convert.ToString(errMsg)
            };
        }

        private static void SetValueForOutputProperty(ICriteria criteria, Hashtable hash)
        {
            IEnumerable<PropertyInfo> properties = criteria.GetType().GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(OutputAttribute)));

            foreach (PropertyInfo property in properties)
            {
                if (property.CanWrite)
                {
                    var value = hash[property.Name.ToLower(CultureInfo.InvariantCulture)];
                    property.SetValue(criteria, value, null);
                }
            }
        }
    }
}