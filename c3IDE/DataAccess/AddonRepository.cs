﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using c3IDE.Models;
using LiteDB;

namespace c3IDE.DataAccess
{
    public class AddonRepository : IRepository<C3Addon>
    {
        public string Database { get; set; } = "data.db";
        public string Collection { get; set; } = "Addons";
        public void Insert(C3Addon value)
        {
            using (var db = new LiteDatabase(Database))
            {
                var collection = db.GetCollection<C3Addon>(Collection);
                collection.Insert(value);
            }
        }

        public void Upsert(C3Addon value)
        {
            using (var db = new LiteDatabase(Database))
            {
                var collection = db.GetCollection<C3Addon>(Collection);
                collection.Upsert(value);
            }
        }

        public void BulkInsert(IEnumerable<C3Addon> values)
        {
            using (var db = new LiteDatabase(Database))
            {
                var collection = db.GetCollection<C3Addon>(Collection);
                collection.Insert(values);
            }
        }

        public void BulkUpsert(IEnumerable<C3Addon> values)
        {
            using (var db = new LiteDatabase(Database))
            {
                var collection = db.GetCollection<C3Addon>(Collection);
                collection.Upsert(values);
            }
        }

        public IEnumerable<C3Addon> GetAll()
        {
            using (var db = new LiteDatabase(Database))
            {
                var collection = db.GetCollection<C3Addon>(Collection);
                return collection.FindAll();
            }
        }

        public IEnumerable<C3Addon> Get(Expression<Func<C3Addon, bool>> predicate)
        {
            using (var db = new LiteDatabase(Database))
            {
                var collection = db.GetCollection<C3Addon>(Collection);
                return collection.Find(predicate);
            }
        }

        public void Delete(C3Addon value)
        {
            using (var db = new LiteDatabase(Database))
            {
                var collection = db.GetCollection<C3Addon>(Collection);
                collection.Delete(value.Id);
            }
        }

        public void ResetCollection()
        {
            using (var db = new LiteDatabase(Database))
            {
                db.DropCollection(Collection);
            }
        }
    }
}