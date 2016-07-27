using System.Collections.Generic;
using BrainShare.Models;


namespace BrainShare.ViewModels
{
    class TopicPageViewModel 
    {
        private string _topicTitle;
        public string TopicName
        {
            get { return _topicTitle; }
            set { _topicTitle = value; }
        }
        private string _notes;
        public string TopicNotes
        {
            get { return _notes; }
            set { _notes = value; }
        }
        private List<AttachmentObservable> _attachments;
        public List<AttachmentObservable> AttachmentList
        {
            get { return _attachments; }
            set { _attachments = value; }
        }
        public TopicPageViewModel(TopicObservable Topic)
        {
            TopicName = Topic.TopicTitle;
            TopicNotes = Topic.notes;
            AttachmentList = Topic.Files;
        }

    }
}
