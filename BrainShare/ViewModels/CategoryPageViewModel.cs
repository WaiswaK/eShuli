using System.Collections.Generic;
using BrainShare.Models;

namespace BrainShare.ViewModels
{
    class CategoryPageViewModel
    {
        private string _categoryName;
        public string CategoryName
        {
            get { return _categoryName; }
            set { _categoryName = value; }
        }
        private List<VideoObservable> _videosList;
        public List<VideoObservable> VideosList
        {
            get { return _videosList; }
            set { _videosList = value; }
        }
        private List<AssignmentObservable> _assignment;
        public List<AssignmentObservable> AssignmentList
        {
            get { return _assignment; }
            set { _assignment = value; }
        }
        public CategoryPageViewModel(CategoryObservable category)
        {
            VideosList = category.videos;
            AssignmentList = category.assignments;
            CategoryName = category.categoryName;
        }
    }
}
