﻿using System.Collections.Generic;

namespace BrainShare.Models
{
    class UserObservable
    {
        public string email { get; set; }
        public string password { get; set; }
        public string full_names { get; set; }
        public SchoolObservable School { get; set; }
        public List<SubjectObservable> subjects { get; set; }
        public int update_status { get; set; }
        public LibraryObservable Library { get; set; }
        public bool NotesImagesDownloading { get; set; }
    }
}
