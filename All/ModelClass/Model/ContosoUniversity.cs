using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClass.Model
{
    public enum Grade
    {
        A, B, C, D, F
    }
    #region Student Class
    public class Student
    {
        private string _address;
        private string _studentName;
        private int _studentId;
        private decimal _age;
        private int _standardId;

        public Student()
        {
        }
        public int ID { get; set; }
        public int StudentID
        {
            get { return _studentId; }
            set { _studentId = value; }
        }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string FirstName { get; set; }
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

        public int StandardID
        {
            get { return _standardId; }
            set { _standardId = value; }
        }
        public DateTime EnrollmentDate { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
    }
    #endregion

    #region Enrollment Class
    public class Enrollment
    {
        public int EnrollmentID { get; set; }
        public int CourseID { get; set; }
        public int StudentID { get; set; }
        public Grade? Grade { get; set; }

        public Course Course { get; set; }
        public Student Student { get; set; }
    }
    #endregion

    #region Course Class
    public class Course
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CourseID { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
    }


    #endregion

    #region Faculty Class
    public class Faculty
    {
        public int Id { get; set; }
        public string FacultyCodeNo { get; set; }
        public string FacultyName { get; set; }
    }
    #endregion

    #region Standard Class
    public class Standard
    {
        public int StandardID { get; set; }
        public string StandardName { get; set; }
    }
    #endregion
}
