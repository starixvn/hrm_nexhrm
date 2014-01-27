// --------------------------------------------------------------------------
// <copyright file="MemoRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.Data;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;

    /// <summary>
    /// Memo repository.
    /// </summary>
    public class MemoRepository : IMemoRepository
    {
        /// <summary>
        /// Get all memo.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="year">The year value.</param>
        /// <param name="category">The category.</param>
        /// <returns>List of memo.</returns>
        public IList<Memo> GetAll(string content, int year, int category)
        {
            using (var db = new ObjectDb("Memo_GetAll"))
            {
                return db.Query<Memo>(new { content = content, year = year, category = category }).ToList();
            }
        }

        /// <summary>
        /// Get memo by ID.
        /// </summary>
        /// <param name="id">The memo ID.</param>
        /// <returns>The memo object.</returns>
        public Memo GetById(int id)
        {
            using (var db = new ObjectDb("Memo_GetById"))
            {
                return db.QueryEntity<Memo>(new { id });
            }
        }

        /// <summary>
        /// Delete memo.
        /// </summary>
        /// <param name="id">The memo ID.</param>
        /// <returns>The result.</returns>
        public bool Delete(int id)
        {
            using (var db = new ObjectDb("Memo_Delete"))
            {
                db.AddInParameter("@Id", DbType.Int32, id);
                return db.ExecuteNonQuery() != 0;
            }
        }

        /// <summary>
        /// Insert memo.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>The result.</returns>
        public int Insert(Memo model)
        {
            using (var db = new ObjectDb("Memo_Insert"))
            {
                db.AddInParameter("@RefNo", DbType.String, model.RefNo);
                db.AddInParameter("@Contents", DbType.String, model.Contents);
                db.AddInParameter("@Subject", DbType.String, model.Subject);
                db.AddInParameter("@SentTo", DbType.String, model.SentTo);
                db.AddInParameter("@SentFrom", DbType.String, model.SentFrom);
                db.AddInParameter("@ForInfo", DbType.String, model.ForInfo);
                db.AddInParameter("@TotalPages", DbType.Int32, model.TotalPages);
                db.AddInParameter("@SentDate", DbType.DateTime, model.SentDate);
                db.AddInParameter("@Category", DbType.Int32, model.Category);
                db.AddInParameter("@Type", DbType.Int32, model.Type);

                return (int)db.ExecuteScalar();
            }
        }

        /// <summary>
        /// Update memo.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>The result.</returns>
        public bool Update(Memo model)
        {
            using (var db = new ObjectDb("Memo_Update"))
            {
                db.AddInParameter("@Id", DbType.Int32, model.Id);
                db.AddInParameter("@RefNo", DbType.String, model.RefNo);
                db.AddInParameter("@Contents", DbType.String, model.Contents);
                db.AddInParameter("@Subject", DbType.String, model.Subject);
                db.AddInParameter("@SentTo", DbType.String, model.SentTo);
                db.AddInParameter("@SentFrom", DbType.String, model.SentFrom);
                db.AddInParameter("@ForInfo", DbType.String, model.ForInfo);
                db.AddInParameter("@TotalPages", DbType.Int32, model.TotalPages);
                db.AddInParameter("@SentDate", DbType.DateTime, model.SentDate);
                db.AddInParameter("@Category", DbType.Int32, model.Category);
                db.AddInParameter("@Type", DbType.Int32, model.Type);

                return db.ExecuteNonQuery() != 0;
            }
        }
    }
}
