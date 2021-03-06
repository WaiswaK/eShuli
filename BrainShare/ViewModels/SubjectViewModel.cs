﻿using System.Collections.Generic;
using BrainShare.Models;


namespace BrainShare.ViewModels
{
    class SubjectViewModel 
    {
        private string _subjectName;
        public string SubjectName
        {
            get { return _subjectName; }
            set { _subjectName = value; }
        }
        #region attachments
        private List<AttachmentModel> _bookList;
        public List<AttachmentModel> FileList
        {
            get { return _bookList; }
            set { _bookList = value; }
        }
        #endregion
        private List<FolderModel> topics;
        public List<FolderModel> TopicList
        {
            get { return topics; }
            set { topics = value; }
        }
        private List<FolderModel> GetFolders(List<TopicModel> topics) 
        {
            List<FolderModel> folders = new List<FolderModel>();
            foreach (var topic in topics)
            {
                List<TopicModel> folderTopics = new List<TopicModel>();
                FolderModel folder = new FolderModel();
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
        private List<VideoModel> _videosList;
        public List<VideoModel> VideosList
        {
            get { return _videosList; }
            set { _videosList = value; }
        }
        private List<AssignmentModel> _assignment;
        public List<AssignmentModel> AssignmentList
        {
            get { return _assignment; }
            set { _assignment = value; }
        }
        public SubjectViewModel(SubjectModel subject)
        {        
            SubjectName = subject.name;
            TopicList = GetFolders(subject.topics);
            FileList = subject.files;
            VideosList = subject.videos;
            AssignmentList = subject.assignments;
        }
    }
}
