using System.Collections.Generic;
using BrainShare.Models;

namespace BrainShare.ViewModels
{
    class TopicsPageViewModel
    {
        private string _topicTitle;
        private string _folderName;
        public string TopicTitle
        {
            get { return _topicTitle; }
            set { _topicTitle = value; }

        }
        public string FolderName
        {
            get { return _folderName; }
            set { _folderName = value; }

        }
        private List<TopicObservable> _topics;
        public List<TopicObservable> TopicList
        {
            get { return _topics; }
            set { _topics = value; }
        }
        #region Attachments
        private List<AttachmentObservable> _attachments;
        public List<AttachmentObservable> AttachmentList
        {
            get { return _attachments; }
            set { _attachments = value; }
        }
        public List<AttachmentObservable> AllAttachments(FolderObservable MainTopic)
        {
            List<AttachmentObservable> attachments = new List<AttachmentObservable>();
            foreach (var topic in MainTopic.topics)
            {
                foreach (var file in topic.Files)
                {
                    attachments.Add(file);
                }
            }
            return attachments;
        }
        #endregion
        public TopicsPageViewModel(FolderObservable MainTopic)
        {
            TopicTitle = MainTopic.folder_name;
            TopicList = MainTopic.topics;
            FolderName = MainTopic.folder_name;
            AttachmentList = AllAttachments(MainTopic);
        }
    }
}
