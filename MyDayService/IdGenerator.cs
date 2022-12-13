using System;
using MongoDB.Bson.Serialization;

public class IdGenerator : IIdGenerator
{
    public object GenerateId(object container, object document)
    {
        return Guid.NewGuid().ToString();
    }

    public bool IsEmpty(object id)
    {
        return id == null || string.IsNullOrEmpty(id.ToString()) || id.Equals("00000000-0000-0000-0000-000000000000");
    }
}