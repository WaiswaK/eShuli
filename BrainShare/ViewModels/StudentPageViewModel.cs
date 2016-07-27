using System.Collections.Generic;
using BrainShare.Models;
using System.Linq;

namespace BrainShare.ViewModels
{
    class StudentPageViewModel{
        //School
        private SchoolObservable _school;
        public SchoolObservable School
        {
            get { return _school; }
            set { _school = value; }
        }
        private string _schoolname;
        public string SchoolName
        {
            get { return _schoolname; }
            set { _schoolname = value; }
        }
        private string _schoolbadge;
        public string SchoolBadge
        {
            get { return _schoolbadge; }
            set { _schoolbadge = value; }
        }
        //Subjects
        private List<SubjectObservable> _courses;
        public List<SubjectObservable> CourseList
        {
            get { return _courses; }
            set { _courses = value; }
        }
        private LibraryObservable _library;
        public LibraryObservable Library
        {
         get { return _library; }
         set { _library = value; }
        }

        private List<Library_CategoryObservable> _libCategory;
        public List<Library_CategoryObservable> CategoryList
        {
            get { return _libCategory; }
            set { _libCategory = value; }
        }
        private List<Library_CategoryObservable> LibraryCategories(UserObservable user)
        {
            List<Library_CategoryObservable> categories = new List<Library_CategoryObservable>();
            List<BookObservable> books = new List<BookObservable>();
            foreach (var category in user.Library.categories)
            {
                category.category_image = category.category_books.First().thumb_url;
                categories.Add(category);
            }

            return categories;
        }

        public StudentPageViewModel(UserObservable user)
        {
            School = user.School;
            SchoolName = School.SchoolName;
            SchoolBadge = School.ImagePath;
            CourseList = user.subjects;
            CategoryList = LibraryCategories(user);

        }
    }
}
