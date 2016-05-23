﻿using BrainShare.Common;
using BrainShare.Database;
using BrainShare.Models;
using System.Collections.Generic;
using System.Linq;

namespace BrainShare.Core
{
    class DatabaseOutputTask
    {
        //Subjects in database
        public static List<Subject> SelectAllSubjects()
        {
            List<Subject> subjects = new List<Subject>();
            List<Subject> nullSubject = null;
            int count = 0;
            using (var db = new SQLite.SQLiteConnection(Constants.dbPath))
            {
                var query = (db.Table<Subject>().ToList());
                subjects = query;
                count = query.Count;
            }
            if (count > 0)
                return subjects;
            else
                return nullSubject;
        }
        //Users in Database
        public static List<UserAccount> SelectAllUsers()
        {
            List<UserAccount> users = new List<UserAccount>();
            List<UserAccount> nullUser = null;
            int count = 0;
            using (var db = new SQLite.SQLiteConnection(Constants.dbPath))
            {
                var query = (db.Table<UserAccount>().ToList());
                users = query;
                count = query.Count;
            }
            if (count > 0)
                return users;
            else
                return nullUser;
        }
        //Method to get Subject Ids of a particular User
        public static List<int> SubjectIdsForUser(string username)
        {
            char[] delimiter = { '.' };
            List<int> subjectids = new List<int>();
            using (var db = new SQLite.SQLiteConnection(Constants.dbPath))
            {
                var query = (db.Table<UserAccount>().Where(c => c.e_mail == username)).Single();
                string[] SplitSubjectId = query.subjects.Split(delimiter);
                List<string> SubjectIdList = SplitSubjectId.ToList();
                subjectids = ModelTask.SubjectIdsConvert(SubjectIdList);
            }
            return subjectids;
        }
        //Method to get Subject details
        public static SubjectObservable GetSubject(int id)
        {
            SubjectObservable sub = new SubjectObservable();
            using (var db = new SQLite.SQLiteConnection(Constants.dbPath))
            {
                var query = (db.Table<Subject>().Where(c => c.SubjectId == id)).Single();
                sub.Id = id;
                sub.name = query.name;
                sub.thumb = query.thumb;
                sub.topics = GetTopics(query.SubjectId);
                sub.videos = GetVideos(query.SubjectId);
                sub.files = GetFiles(0, query.SubjectId, 0);
                sub.assignments = GetAssignments(query.SubjectId);
            }
            return sub;
        }
        public static SchoolObservable GetSchool(int school_id)
        {
            SchoolObservable school = new SchoolObservable();
            using (var db = new SQLite.SQLiteConnection(Constants.dbPath))
            {
                var query = (db.Table<School>().Where(c => c.School_id == school_id)).Single();
                school = new SchoolObservable(query.SchoolName, query.SchoolBadge, query.School_id);
            }
            return school;
        }
        private static List<TopicObservable> GetTopics(int subId)
        {
            List<TopicObservable> topics = new List<TopicObservable>();
            TopicObservable topic = null;
            using (var db = new SQLite.SQLiteConnection(Constants.dbPath))
            {
                var query = (db.Table<Topic>().Where(c => c.SubjectId == subId));
                foreach (var _topic in query)
                {
                    topic = new TopicObservable(_topic.TopicID, _topic.Notes, _topic.Updated_Notes, _topic.TopicTitle, GetFiles(_topic.TopicID, 0, 0), _topic.teacher_full_names, _topic.Updated_at, _topic.Folder_Id, _topic.Folder_Name);
                    topics.Add(topic);
                }
            }
            return topics;
        }
        public static List<AttachmentObservable> GetFiles(int id1, int id2, int id3)
        {
            List<AttachmentObservable> attachments = new List<AttachmentObservable>();
            AttachmentObservable attachment = null;
            using (var db = new SQLite.SQLiteConnection(Constants.dbPath))
            {
                if (id1 > 0)
                {
                    var query = (db.Table<Attachment>().Where(c => c.TopicID == id1));
                    foreach (var _file in query)
                    {
                        attachment = new AttachmentObservable(_file.AttachmentID, _file.FilePath, _file.FileName);
                        attachments.Add(attachment);
                    }
                }
                if (id2 > 0)
                {
                    var query = (db.Table<Attachment>().Where(c => c.SubjectId == id2));
                    foreach (var _file in query)
                    {
                        attachment = new AttachmentObservable(_file.AttachmentID, _file.FilePath, _file.FileName);
                        attachments.Add(attachment);
                    }
                }
                if (id3 > 0)
                {
                    var query = (db.Table<Attachment>().Where(c => c.AssignmentID == id3));
                    foreach (var _file in query)
                    {
                        attachment = new AttachmentObservable(_file.AttachmentID, _file.FilePath, _file.FileName);
                        attachments.Add(attachment);
                    }
                }
            }
            return attachments;
        }
        private static List<VideoObservable> GetVideos(int Subjectid)
        {
            List<VideoObservable> videos = new List<VideoObservable>();
            VideoObservable video = null;
            using (var db = new SQLite.SQLiteConnection(Constants.dbPath))
            {
                var query = (db.Table<Video>().Where(c => c.SubjectId == Subjectid));
                foreach (var _video in query)
                {
                    video = new VideoObservable(_video.VideoID, _video.FilePath, _video.FileName, _video.description, _video.teacher_full_names);
                    videos.Add(video);
                }
            }
            return videos;
        }
        private static List<AssignmentObservable> GetAssignments(int subId)
        {
            List<AssignmentObservable> assignments = new List<AssignmentObservable>();
            AssignmentObservable assignment = null;
            using (var db = new SQLite.SQLiteConnection(Constants.dbPath))
            {
                var query = (db.Table<Assignment>().Where(c => c.SubjectId == subId));
                foreach (var _assignment in query)
                {
                    assignment = new AssignmentObservable(_assignment.AssignmentID, _assignment.title, _assignment.description, _assignment.teacher_full_names, GetFiles(0, 0, _assignment.AssignmentID));
                    assignments.Add(assignment);
                }
            }
            return assignments;
        }
        public static List<AttachmentObservable> OldGetFiles(int topicID, int assignmentID)
        {
            List<AttachmentObservable> attachments = new List<AttachmentObservable>();
            AttachmentObservable attachment = null;

            if (topicID == 0 && assignmentID > 0)
            {
                using (var db = new SQLite.SQLiteConnection(Constants.dbPath))
                {
                    var query = (db.Table<Attachment>().Where(c => c.AssignmentID == assignmentID));
                    foreach (var _title in query)
                    {
                        attachment = new AttachmentObservable(_title.AttachmentID, _title.FilePath, _title.FileName);
                        attachments.Add(attachment);
                    }
                }
            }

            if (topicID > 0 && assignmentID == 0)
            {
                using (var db = new SQLite.SQLiteConnection(Constants.dbPath))
                {
                    var query = (db.Table<Attachment>().Where(c => c.TopicID == topicID));
                    foreach (var _title in query)
                    {
                        attachment = new AttachmentObservable(_title.AttachmentID, _title.FilePath, _title.FileName);
                        attachments.Add(attachment);
                    }
                }
            }
            return attachments;
        }
        public static LibraryObservable GetLibrary(int school_id)
        {
            LibraryObservable library = new LibraryObservable();
            List<Book> books = new List<Book>();
            List<Library_CategoryObservable> categories = new List<Library_CategoryObservable>();
            int count;
            using (var db = new SQLite.SQLiteConnection(Constants.dbPath))
            {
                var query = db.Table<Book>().Where(c => c.Library_id == school_id);
                books = query.ToList();
                count = books.Count;
            }
            library.library_id = school_id;
            library.categories = ModelTask.categories(books);
            return library;
        }
    }
}
