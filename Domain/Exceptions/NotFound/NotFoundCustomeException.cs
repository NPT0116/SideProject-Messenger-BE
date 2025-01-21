using System;

namespace Domain.Exceptions.NotFound;


    public class NotFoundCustomException<T> : Exception
    {
        public NotFoundCustomException(Guid id)
            : base($"{typeof(T).Name} with ID {id} was not found.")
        {
            EntityType = typeof(T).Name;
            EntityId = id;
        }

        public NotFoundCustomException(string identifier)
            : base($"{typeof(T).Name} with identifier {identifier} was not found.")
        {
            EntityType = typeof(T).Name;
            Identifier = identifier;
        }

        public string EntityType { get; }
        public Guid? EntityId { get; }
        public string Identifier { get; }
    }