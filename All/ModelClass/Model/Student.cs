namespace LinqDemo
{
    public class Student
    {
        private string _address;
        private string _studentName;
        private int _studentId;
        private decimal _age;

        public Student()
        {
            
        }
        public int StudentID
        {
            get { return _studentId; }
            set { _studentId = value; }
        }

        public string StudentName
        {
            get { return _studentName; }
            set { _studentName = value; }
        }
        public decimal Age
        {
            get { return _age; }
            set { _age = value; }
        }

        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }
    }
}