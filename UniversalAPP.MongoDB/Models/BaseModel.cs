﻿using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UniversalAPP.MongoDB.Models
{
    /// <summary>
    /// Model基类
    /// </summary>
    public class BaseModel
    {

        [BsonId]        //标记主键
        [BsonRepresentation(BsonType.ObjectId)]     //参数类型  ， 无需赋值
        public string Id { get; set; }

        [BsonElement(nameof(CreateDateTime))]   //指明数据库中字段名为CreateDateTime
        public DateTime CreateDateTime { get; set; }

        //[BsonElement(nameof(IsDelete))]
        public bool IsDelete { get; set; }

        public BaseModel()
        {
            CreateDateTime = DateTime.Now;
            IsDelete = false;
        }

    }
}
