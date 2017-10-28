using System;

namespace ModelClass.Model
{
    public class Address
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public bool IsVerified { get; set; }
        public DateTime DateLastVerified { get; set; }
        public bool? IsPrimary { get; set; }
    }
}
