﻿using System.Collections.Generic;

namespace BrainShare.Models
{
    class FolderModel
    {
        public int folder_id { get; set; }
        public string folder_name { get; set;}
        public List<TopicModel> topics { get; set; }
        public FolderModel() { }
        public FolderModel(int id, string name, List<TopicModel> _topics)
        {
            folder_id = id;
            folder_name = name;
            topics = _topics;
        }
    }
}
