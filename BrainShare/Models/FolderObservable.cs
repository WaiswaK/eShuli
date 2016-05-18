﻿using System.Collections.Generic;

namespace BrainShare.Models
{
    class FolderObservable
    {
        public int folder_id { get; set; }
        public string folder_name { get; set;}
        public List<TopicObservable> topics { get; set; }
        public FolderObservable() { }
        public FolderObservable(int id, string name, List<TopicObservable> _topics)
        {
            folder_id = id;
            folder_name = name;
            topics = _topics;
        }
    }
}
