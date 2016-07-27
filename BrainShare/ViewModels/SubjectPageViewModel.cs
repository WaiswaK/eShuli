﻿using System.Collections.Generic;
using BrainShare.Models;


namespace BrainShare.ViewModels
{
    class SubjectPageViewModel 
    {
        private string _subjectName;
        public string SubjectName
        {
            get { return _subjectName; }
            set { _subjectName = value; }
        }
        private List<CategoryObservable> _category;
        public List<CategoryObservable> CategoryList
        {
            get { return _category; }
            set { _category = value; }
        }
        private List<AttachmentObservable> _bookList;
        public List<AttachmentObservable> FileList
        {
            get { return _bookList; }
            set { _bookList = value; }
        }
        private List<FolderObservable> topics;
        public List<FolderObservable> TopicList
        {
            get { return topics; }
            set { topics = value; }
        }
        private List<FolderObservable> GetFolders(List<TopicObservable> topics) 
        {
            List<FolderObservable> folders = new List<FolderObservable>();
            foreach (var topic in topics)
            {
                List<TopicObservable> folderTopics = new List<TopicObservable>();
                FolderObservable folder = new FolderObservable();
                bool finished = false;
                folder.folder_id = topic.folder_id;
                folder.folder_name = topic.folder_name;
                foreach (var comparisonTopic in topics)
                {
                    finished = false;
                    if (folder.folder_id == comparisonTopic.folder_id)
                    {
                        if (topic.TopicID == comparisonTopic.TopicID)
                        {
                            if (folderTopics.Count > 0)
                            {
                                finished = true;
                            }
                            else
                            {
                                folderTopics.Add(topic);
                            }
                        }
                        else
                        {
                            if (folderTopics.Count > 0)
                            {
                                foreach (var CheckTopic in folderTopics)
                                {
                                    if (comparisonTopic.TopicID == CheckTopic.TopicID)
                                    {
                                        finished = true;
                                    }
                                }
                            }
                            if (finished == false)
                            {
                                folderTopics.Add(comparisonTopic);
                            }
                        }
                    }
                }
                folder.topics = folderTopics;
                finished = false;
                if (folders.Count > 0) 
                {
                    foreach (var folderEntered in folders)
                    {
                        if (folderEntered.folder_id == folder.folder_id)
                        {
                            finished = true;
                        }
                        else
                        {
                            ;
                        }
                    }
                    if (finished == false)
                    {
                        folders.Add(folder);
                        finished = false;
                    }
                }
                else
                {
                    folders.Add(folder);
                }
            }
            return folders;
        }
        private List<CategoryObservable> SortedCategories(SubjectObservable subject)
        {
            List<CategoryObservable> categories = new List<CategoryObservable>();
            CategoryObservable videos = new CategoryObservable("Videos", "ms-appx:///Assets/icons/video-library.jpg", subject.videos.Count, subject.videos);
            CategoryObservable assignments = new CategoryObservable("Assignments", "ms-appx:///Assets/icons/assignment.jpg", subject.assignments.Count, subject.assignments);
            categories.Add(videos);
            categories.Add(assignments);
            if (subject.assignments.Count == 0)
            {
                categories.Remove(assignments);
            }
            if (subject.videos.Count == 0)
            {
                categories.Remove(videos);
            }
            return categories;
        }
        public SubjectPageViewModel(SubjectObservable subject)
        {          
            SubjectName = subject.name;
            CategoryList = SortedCategories(subject);
            TopicList = GetFolders(subject.topics);
            FileList = subject.files;
        }
    }
}
