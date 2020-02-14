using System;


namespace Status
{
    public class ApplicationId 
    {
        public readonly int Id;

        public ApplicationId()
        {
            Id = 0;
        }

        public ApplicationId(int id)
        {
            Id = id;
        }


        public override string ToString()
        {
            return Id.ToString();
        }
        public int CompareTo(ApplicationId other)
        {
            throw new NotImplementedException();
        }

       
    }
}
