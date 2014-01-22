// --------------------------------------------------------------------------
// <copyright file="TOEICRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.05.31">Created</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System.Data;
    using System.Linq;
    using Fanex.Data;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interface;

    /// <summary>
    /// TOEIC repository.
    /// </summary>
    public class TOEICRepository : ITOEICRepository
    {
        /// <summary>
        /// Get TOEIC of specific employee.
        /// </summary>
        /// <param name="empId">Id of employee.</param>
        /// <returns>TOEIC object.</returns>
        public TOEIC GetById(int empId)
        {
            using (ObjectDb db = new ObjectDb("TOEIC_GetScore"))
            {
                return db.Query<TOEIC>(new { empId }).FirstOrDefault();
            }
        }

        /// <summary>
        /// Update new TOEIC score.
        /// </summary>
        /// <param name="model">TOEIC object.</param>
        /// <returns>Effected row.</returns>
        public int Update(TOEIC model)
        {
            using (ObjectDb db = new ObjectDb("TOEIC_Update"))
            {
                db.AddInParameter("@empId", DbType.Int32, model.EmpId);
                db.AddInParameter("@listening", DbType.Int32, model.Listening);
                db.AddInParameter("@reading", DbType.Int32, model.Reading);
                db.AddInParameter("@lrExamDate", DbType.DateTime, model.LRExamDate);
                db.AddInParameter("@oldListening", DbType.Int32, model.OldListening);
                db.AddInParameter("@oldReading", DbType.Int32, model.OldReading);

                db.AddInParameter("@speaking", DbType.Int32, model.Speaking);
                db.AddInParameter("@writing", DbType.Int32, model.Writing);
                db.AddInParameter("@swExamDate", DbType.DateTime, model.SWExamDate);
                db.AddInParameter("@oldSpeaking", DbType.Int32, model.OldSpeaking);
                db.AddInParameter("@oldWriting", DbType.Int32, model.OldWriting);

                db.AddInParameter("@inputDate", DbType.DateTime, model.InputDate);

                db.AddInParameter("@createdBy", DbType.Int32, model.CreatedBy);
                db.AddInParameter("@createdDate", DbType.DateTime, model.CreatedDate);

                db.AddInParameter("@modifiedBy", DbType.Int32, model.ModifiedBy);
                db.AddInParameter("@modifiedDate", DbType.DateTime, model.ModifiedDate);

                // exec
                db.ExecuteNonQuery();

                return 0; // success
            }
        }

        /// <summary>
        /// Update new TOEIC score.
        /// </summary>
        /// <param name="model">TOEIC object.</param>
        /// <returns>Effected row.</returns>
        public int NewUpdate(TOEIC model)
        {
            using (ObjectDb db = new ObjectDb("TOEIC_NewUpdate"))
            {
                db.AddInParameter("@empId", DbType.Int32, model.EmpId);
                db.AddInParameter("@listening", DbType.Int32, model.Listening);
                db.AddInParameter("@reading", DbType.Int32, model.Reading);
                db.AddInParameter("@lrExamDate", DbType.DateTime, model.LRExamDate);

                db.AddInParameter("@speaking", DbType.Int32, model.Speaking);
                db.AddInParameter("@writing", DbType.Int32, model.Writing);
                db.AddInParameter("@swExamDate", DbType.DateTime, model.SWExamDate);

                db.AddInParameter("@inputDate", DbType.DateTime, model.InputDate);

                db.AddInParameter("@createdBy", DbType.Int32, model.CreatedBy);
                db.AddInParameter("@createdDate", DbType.DateTime, model.CreatedDate);

                db.AddInParameter("@modifiedBy", DbType.Int32, model.ModifiedBy);
                db.AddInParameter("@modifiedDate", DbType.DateTime, model.ModifiedDate);

                // exec
                db.ExecuteNonQuery();

                return 0; // success
            }
        }
    }
}
