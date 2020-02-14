using System;

namespace Status
{
    public class ResourceId
    {
        public readonly int Id;
        public ResourceId()
        {
            Id = 0;
        }

        public ResourceId(int id)
        {
            Id = id;
        }


        public override string ToString()
        {
            return Id.ToString();
        }
        public int CompareTo(ResourceId other)
        {
            throw new NotImplementedException();
        }

    }
}