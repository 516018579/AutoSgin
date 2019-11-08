using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSgin.DB.Domain
{
    [Serializable]
    public abstract class Entity<TPrimaryKey> : IEntity<TPrimaryKey>
    {
        /// <summary>Unique identifier for this entity.</summary>
        public virtual TPrimaryKey Id { get; set; }
    }

    [Serializable]
    public abstract class Entity : Entity<int>, IEntity
    {
    }
}
