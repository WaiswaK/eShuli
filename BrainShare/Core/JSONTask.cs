﻿using BrainShare.Common;
using BrainShare.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace BrainShare.Core
{
    class JSONTask
    {
        private static string Teacher(JsonObject obj)
        {
            string teacher_names = string.Empty;
            foreach (var teacher in obj.Keys)
            {
                IJsonValue teacherVal;
                if (!obj.TryGetValue(teacher, out teacherVal))
                    continue;
                switch (teacher)
                {
                    case "full_name":
                        teacher_names = teacherVal.GetString();
                        break;
                }
            }
            return teacher_names;
        }
        #region Login JSON Methods
        public static LoginStatus Notification(JsonObject loginObject)
        {
            LoginStatus user = new LoginStatus();
            foreach (var log in loginObject.Keys)
            {
                IJsonValue val;
                if (!loginObject.TryGetValue(log, out val))
                    continue;
                switch (log)
                {
                    case "statusCode":
                        user.statusCode = val.GetString();
                        break;
                    case "statusDescription":
                        user.statusDescription = val.GetString();
                        break;
                }
            }
            return user;
        }
        public static SchoolObservable GetSchool(JsonObject loginObject)
        {
            SchoolObservable user = new SchoolObservable();
            foreach (var log in loginObject.Keys)
            {
                IJsonValue val;
                if (!loginObject.TryGetValue(log, out val))
                    continue;
                switch (log)
                {
                    case "school_id":
                        user.SchoolId = (int)val.GetNumber();
                        break;
                    case "school":
                        user.SchoolName = val.GetString();
                        break;
                    case "school_logo":
                        user.ImagePath = Constants.BaseUri + val.GetString();
                        break;
                }
            }
            return user;
        }
        public static string GetUsername(JsonObject loginObject)
        {
            string user = string.Empty;
            foreach (var log in loginObject.Keys)
            {
                IJsonValue val;
                if (!loginObject.TryGetValue(log, out val))
                    continue;
                switch (log)
                {
                    case "name":
                        user = val.GetString();
                        break;
                }
            }
            return user;
        }
        public static async Task<JsonObject> LoginJsonObject(string username, string password)
        {
            JsonObject loginObject = new JsonObject();
            var client = new HttpClient();
            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("email", username));
            postData.Add(new KeyValuePair<string, string>("password", password));
            var formContent = new FormUrlEncodedContent(postData);
            var authresponse = await client.PostAsync("http://eshuli.rw/liveapis/authenticate.json", formContent);
            var authresult = await authresponse.Content.ReadAsStreamAsync();
            var authstreamReader = new System.IO.StreamReader(authresult);
            var authresponseContent = authstreamReader.ReadToEnd().Trim().ToString();
            loginObject = JsonObject.Parse(authresponseContent);
            return loginObject;
        }
        #endregion
        #region Subjects JSON Methods
        public static SubjectObservable GetSubject(JsonArray SubjectsArray, int Sub_id, JsonArray NotesArray, JsonArray VideosArray, JsonArray AssignmentArray, JsonArray FilesArray)
        {
            SubjectObservable subject = new SubjectObservable();
            int temp = 0;
            foreach (var item in SubjectsArray)
            {
                var obj = item.GetObject();
                foreach (var key in obj.Keys)
                {
                    IJsonValue val;
                    if (!obj.TryGetValue(key, out val))
                        continue;
                    switch (key)
                    {
                        case "id":
                            temp = (int)val.GetNumber();
                            break;
                        case "name":
                            if (temp == Sub_id)
                            {
                                subject.Id = Sub_id;
                                subject.name = val.GetString();
                                subject.thumb = "ms-appx:///Assets/Course/course.jpg";
                                var noteslist = (from i in NotesArray select i.GetObject()).ToList();
                                subject.topics = GetTopics(noteslist);
                                var videoslist = (from i in VideosArray select i.GetObject()).ToList();
                                subject.videos = GetVideos(videoslist);
                                var fileslist = (from i in FilesArray select i.GetObject()).ToList();
                                subject.files = GetFiles(fileslist);
                                var assignmentlist = (from i in AssignmentArray select i.GetObject()).ToList();
                                subject.assignments = GetAssignments(assignmentlist);
                            }
                            break;
                    }
                }
            }
            return subject;
        }
        public static List<int> SubjectIds(JsonArray SubjectsArray)
        {
            int id;
            List<int> ids = new List<int>();
            foreach (var item in SubjectsArray)
            {
                var obj = item.GetObject();
                foreach (var key in obj.Keys)
                {
                    IJsonValue val;
                    if (!obj.TryGetValue(key, out val))
                        continue;
                    switch (key)
                    {
                        case "id":
                            id = (int)val.GetNumber();
                            ids.Add(id);
                            break;
                    }
                }
            }
            return ids;
        }
        private static List<AttachmentObservable> AllFiles(List<JsonObject> AllObjects)
        {
            List<AttachmentObservable> Files = new List<AttachmentObservable>();
            foreach (var SingleObject in AllObjects)
            {
                AttachmentObservable file = SingleFile(SingleObject);
                Files.Add(file);
            }
            return Files;
        }
        private static AttachmentObservable SingleFile(JsonObject obj)
        {
            AttachmentObservable attachment = new AttachmentObservable();
            foreach (var key in obj.Keys)
            {
                IJsonValue val;
                if (!obj.TryGetValue(key, out val))
                    continue;
                switch (key)
                {
                    case "id":
                        attachment.AttachmentID = (int)val.GetNumber();
                        break;
                    case "name":
                        attachment.FileName = val.GetString();
                        break;
                    case "absolute_uri":
                        attachment.FilePath = val.GetString();
                        break;
                }
            }
            return attachment;
        }        
        public static async Task <List<SubjectObservable>> Get_Subjects(string username, string password,List<int> remainedIDs, List<int> IDs, JsonArray subjects)
        {
            List<int> UpdateIds = ModelTask.oldIds(remainedIDs, IDs);
            List<SubjectObservable> oldcourses = new List<SubjectObservable>();

            foreach (var id in UpdateIds)
            {
                var notes_httpclient = new HttpClient();
                var notes_postData = new List<KeyValuePair<string, string>>();
                notes_postData.Add(new KeyValuePair<string, string>("email", username));
                notes_postData.Add(new KeyValuePair<string, string>("password", password));
                notes_postData.Add(new KeyValuePair<string, string>("id", id.ToString()));
                var notes_formContent = new FormUrlEncodedContent(notes_postData);
                var notes_response = await notes_httpclient.PostAsync("http://eshuli.rw/liveapis/uni_notes.json", notes_formContent);
                var notes_result = await notes_response.Content.ReadAsStreamAsync();
                var notes_streamReader = new System.IO.StreamReader(notes_result);
                var notes_responseContent = notes_streamReader.ReadToEnd().Trim().ToString();
                var notes = JsonArray.Parse(notes_responseContent);

                var videos_httpclient = new HttpClient();
                var videospostData = new List<KeyValuePair<string, string>>();
                videospostData.Add(new KeyValuePair<string, string>("email", username));
                videospostData.Add(new KeyValuePair<string, string>("password", password));
                videospostData.Add(new KeyValuePair<string, string>("id", id.ToString()));
                var videosformContent = new FormUrlEncodedContent(videospostData);
                var videosresponse = await videos_httpclient.PostAsync("http://eshuli.rw/liveapis/uni_videos.json", videosformContent);
                var videosresult = await videosresponse.Content.ReadAsStreamAsync();
                var videosstreamReader = new System.IO.StreamReader(videosresult);
                var videosresponseContent = videosstreamReader.ReadToEnd().Trim().ToString();
                var videos = JsonArray.Parse(videosresponseContent);

                var assgnmt_httpclient = new HttpClient();
                var assgnmt_postData = new List<KeyValuePair<string, string>>();
                assgnmt_postData.Add(new KeyValuePair<string, string>("email", username));
                assgnmt_postData.Add(new KeyValuePair<string, string>("password", password));
                assgnmt_postData.Add(new KeyValuePair<string, string>("id", id.ToString()));
                var assgnmt_formContent = new FormUrlEncodedContent(assgnmt_postData);
                var assgnmt_response = await assgnmt_httpclient.PostAsync("http://eshuli.rw/liveapis/assignments.json", assgnmt_formContent);
                var assgnmt_result = await assgnmt_response.Content.ReadAsStreamAsync();
                var assgnmt_streamReader = new System.IO.StreamReader(assgnmt_result);
                var assgnmt_responseContent = assgnmt_streamReader.ReadToEnd().Trim().ToString();
                var assignments = JsonArray.Parse(assgnmt_responseContent);

                var file_httpclient = new HttpClient();
                var file_postData = new List<KeyValuePair<string, string>>();
                file_postData.Add(new KeyValuePair<string, string>("email", username));
                file_postData.Add(new KeyValuePair<string, string>("password", password));
                file_postData.Add(new KeyValuePair<string, string>("id", id.ToString()));
                var file_formContent = new FormUrlEncodedContent(file_postData);
                var file_response = await file_httpclient.PostAsync("http://eshuli.rw/liveapis/uni_files.json", file_formContent);
                var file_result = await file_response.Content.ReadAsStreamAsync();
                var file_streamReader = new System.IO.StreamReader(file_result);
                var file_responseContent = file_streamReader.ReadToEnd().Trim().ToString();
                var files = JsonArray.Parse(file_responseContent);

                SubjectObservable subject = GetSubject(subjects, id, notes, videos, assignments, files);
                oldcourses.Add(subject);
            }
            return oldcourses;
        }
        public static async Task<List<SubjectObservable>> Get_New_Subjects(string username, string password, List<int> NewSubjectIds, JsonArray subjects)
        {
            List<SubjectObservable> CurrentSubjects = new List<SubjectObservable>();
            foreach (var id in NewSubjectIds)
            {
                var notes_httpclient = new HttpClient();
                var notes_postData = new List<KeyValuePair<string, string>>();
                notes_postData.Add(new KeyValuePair<string, string>("email", username));
                notes_postData.Add(new KeyValuePair<string, string>("password", password));
                notes_postData.Add(new KeyValuePair<string, string>("id", id.ToString()));
                var notes_formContent = new FormUrlEncodedContent(notes_postData);
                var notes_response = await notes_httpclient.PostAsync("http://eshuli.rw/liveapis/uni_notes.json", notes_formContent);
                var notes_result = await notes_response.Content.ReadAsStreamAsync();
                var notes_streamReader = new System.IO.StreamReader(notes_result);
                var notes_responseContent = notes_streamReader.ReadToEnd().Trim().ToString();
                var notes = JsonArray.Parse(notes_responseContent);

                var videos_httpclient = new HttpClient();
                var videospostData = new List<KeyValuePair<string, string>>();
                videospostData.Add(new KeyValuePair<string, string>("email", username));
                videospostData.Add(new KeyValuePair<string, string>("password", password));
                videospostData.Add(new KeyValuePair<string, string>("id", id.ToString()));
                var videosformContent = new FormUrlEncodedContent(videospostData);
                var videosresponse = await videos_httpclient.PostAsync("http://eshuli.rw/liveapis/uni_videos.json", videosformContent);
                var videosresult = await videosresponse.Content.ReadAsStreamAsync();
                var videosstreamReader = new System.IO.StreamReader(videosresult);
                var videosresponseContent = videosstreamReader.ReadToEnd().Trim().ToString();
                var videos = JsonArray.Parse(videosresponseContent);

                var assgnmt_httpclient = new HttpClient();
                var assgnmt_postData = new List<KeyValuePair<string, string>>();
                assgnmt_postData.Add(new KeyValuePair<string, string>("email", username));
                assgnmt_postData.Add(new KeyValuePair<string, string>("password", password));
                assgnmt_postData.Add(new KeyValuePair<string, string>("id", id.ToString()));
                var assgnmt_formContent = new FormUrlEncodedContent(assgnmt_postData);
                var assgnmt_response = await assgnmt_httpclient.PostAsync("http://eshuli.rw/liveapis/assignments.json", assgnmt_formContent);
                var assgnmt_result = await assgnmt_response.Content.ReadAsStreamAsync();
                var assgnmt_streamReader = new System.IO.StreamReader(assgnmt_result);
                var assgnmt_responseContent = assgnmt_streamReader.ReadToEnd().Trim().ToString();
                var assignments = JsonArray.Parse(assgnmt_responseContent);

                var file_httpclient = new HttpClient();
                var file_postData = new List<KeyValuePair<string, string>>();
                file_postData.Add(new KeyValuePair<string, string>("email", username));
                file_postData.Add(new KeyValuePair<string, string>("password", password));
                file_postData.Add(new KeyValuePair<string, string>("id", id.ToString()));
                var file_formContent = new FormUrlEncodedContent(file_postData);
                var file_response = await file_httpclient.PostAsync("http://eshuli.rw/liveapis/uni_files.json", file_formContent);
                var file_result = await file_response.Content.ReadAsStreamAsync();
                var file_streamReader = new System.IO.StreamReader(file_result);
                var file_responseContent = file_streamReader.ReadToEnd().Trim().ToString();
                var files = JsonArray.Parse(file_responseContent);

                SubjectObservable subject = GetSubject(subjects, id, notes, videos, assignments, files);
                CurrentSubjects.Add(subject);
            }
            return CurrentSubjects;
        }        
        public static async Task<JsonArray> SubjectsJsonArray(string username, string password)
        {
            JsonArray subjects = new JsonArray();
            var units_http_client = new HttpClient();
            var units_postData = new List<KeyValuePair<string, string>>();
            units_postData.Add(new KeyValuePair<string, string>("email", username));
            units_postData.Add(new KeyValuePair<string, string>("password", password));
            var units_formContent = new FormUrlEncodedContent(units_postData);
            var courseresponse = await units_http_client.PostAsync("http://eshuli.rw/liveapis/course_units.json", units_formContent);
            var coursesresult = await courseresponse.Content.ReadAsStreamAsync();
            var coursestreamReader = new System.IO.StreamReader(coursesresult);
            var courseresponseContent = coursestreamReader.ReadToEnd().Trim().ToString();
            subjects = JsonArray.Parse(courseresponseContent);
            return subjects;
        }
        #endregion
        #region Topics JSON Methods
        private static List<TopicObservable> GetTopics(List<JsonObject> AllObjects)
        {
            List<TopicObservable> Topics = new List<TopicObservable>();
            foreach (var SingleObject in AllObjects)
            {
                TopicObservable topic = SingleTopic(SingleObject);
                Topics.Add(topic);
            }
            return Topics;
        }
        private static TopicObservable SingleTopic(JsonObject obj)
        {
            TopicObservable topic = new TopicObservable();
            foreach (var key in obj.Keys)
            {
                IJsonValue val;
                if (!obj.TryGetValue(key, out val))
                    continue;
                switch (key)
                {
                    case "id":
                        topic.TopicID = (int)val.GetNumber();
                        break;
                    case "title":
                        topic.TopicTitle = val.GetString();
                        break;
                    case "updated_at":
                        topic.Updated_at = val.GetString();
                        break;
                    case "body":
                        topic.body = val.GetString();
                        break;
                    case "folder_name":
                        topic.folder_name = val.GetString();
                        break;
                    case "folder_id":
                        topic.folder_id = (int)val.GetNumber();
                        break;
                    case "teacher":
                        var teacherObject = val.GetObject();
                        topic.teacher = Teacher(teacherObject);
                        break;
                    case "attachments":
                        var attachmentArray = val.GetArray();
                        var list = (from i in attachmentArray select i.GetObject()).ToList();
                        topic.Files = AllFiles(list);
                        break;
                }
            }
            return topic;
        }
        private static List<VideoObservable> GetVideos(List<JsonObject> AllObjects)
        {
            List<VideoObservable> videos = new List<VideoObservable>();
            foreach (var SingleObject in AllObjects)
            {
                VideoObservable video = SingleVideo(SingleObject);
                videos.Add(video);
            }
            return videos;
        }
        private static VideoObservable SingleVideo(JsonObject obj)
        {
            VideoObservable video = new VideoObservable();
            foreach (var key in obj.Keys)
            {
                IJsonValue val;
                if (!obj.TryGetValue(key, out val))
                    continue;
                switch (key)
                {
                    case "id":
                        video.VideoID = (int)val.GetNumber();
                        break;
                    case "title":
                        video.FileName = val.GetString();
                        break;
                    case "link":
                        video.FilePath = val.GetString();
                        break;
                    case "description":
                        video.description = val.GetString();
                        break;
                    case "teacher":
                        var teacherObject = val.GetObject();
                        video.teacher = Teacher(teacherObject);
                        break;
                }
            }
            return video;
        }
        private static List<AssignmentObservable> GetAssignments(List<JsonObject> AllObjects)
        {
            List<AssignmentObservable> Assignments = new List<AssignmentObservable>();
            foreach (var SingleObject in AllObjects)
            {
                AssignmentObservable Assignment = SingleAssignment(SingleObject);
                Assignments.Add(Assignment);
            }
            return Assignments;
        }
        private static AssignmentObservable SingleAssignment(JsonObject obj)
        {
            AssignmentObservable assignment = new AssignmentObservable();
            foreach (var key in obj.Keys)
            {
                IJsonValue val;
                if (!obj.TryGetValue(key, out val))
                    continue;
                switch (key)
                {
                    case "id":
                        assignment.id = (int)val.GetNumber();
                        break;
                    case "title":
                        assignment.title = val.GetString();
                        break;
                    case "description":
                        assignment.description = val.GetString();
                        break;
                    case "teacher":
                        var teacherObject = val.GetObject();
                        assignment.teacher = Teacher(teacherObject);
                        break;
                    case "attachments":
                        var attachmentArray = val.GetArray();
                        var list = (from i in attachmentArray select i.GetObject()).ToList();
                        assignment.Files = AllFiles(list);
                        break;
                }
            }
            return assignment;
        }
        private static List<AttachmentObservable> GetFiles(List<JsonObject> AllObjects)
        {
            List<AttachmentObservable> files = new List<AttachmentObservable>();
            foreach (var SingleObject in AllObjects)
            {
                AttachmentObservable file = AFile(SingleObject);
                files.Add(file);
            }
            return files;
        }
        private static AttachmentObservable AFile(JsonObject obj)
        {
            AttachmentObservable attachment = new AttachmentObservable();
            foreach (var key in obj.Keys)
            {
                IJsonValue val;
                if (!obj.TryGetValue(key, out val))
                    continue;
                switch (key)
                {
                    case "id":
                        attachment.AttachmentID = (int)val.GetNumber();
                        break;
                    case "name":
                        attachment.FileName = val.GetString();
                        break;
                    case "url":
                        attachment.FilePath = val.GetString();
                        break;
                }
            }
            return attachment;
        }
        #endregion
        #region Library JSON Methods
        private static List<Library_CategoryObservable> GetLibraryCategories(List<JsonObject> AllObjects, int library_id)
        {
            List<Library_CategoryObservable> categories = new List<Library_CategoryObservable>();
            foreach (var SingleObject in AllObjects)
            {
                Library_CategoryObservable category = LibraryCategory(SingleObject, library_id);
                categories.Add(category);
            }
            return categories;
        }
        private static Library_CategoryObservable LibraryCategory(JsonObject obj, int Library_id)
        {
            List<BookObservable> tempbooks = new List<BookObservable>();
            Library_CategoryObservable category = new Library_CategoryObservable();
            foreach (var key in obj.Keys)
            {
                IJsonValue val;
                if (!obj.TryGetValue(key, out val))
                    continue;
                switch (key)
                {
                    case "id":
                        category.category_id = (int)val.GetNumber();
                        int cat_id = (int)val.GetNumber();
                        break;
                    case "name":
                        category.category_name = val.GetString();
                        break;
                    case "book_count":
                        category.book_count = (int)val.GetNumber();
                        break;
                    case "books":
                        var BooksArray = val.GetArray();
                        var BookList = (from i in BooksArray select i.GetObject()).ToList();
                        category.category_books = GetBooks(BookList, Library_id, category.category_id, category.category_name);
                        break;
                }
            }
            return category;
        }
        private static List<BookObservable> GetBooks(List<JsonObject> AllObjects, int lib_id, int category_id, string category_name)
        {
            List<BookObservable> books = new List<BookObservable>();
            foreach (var SingleObject in AllObjects)
            {
                BookObservable Book = SingleBook(SingleObject, lib_id, category_id, category_name);
                books.Add(Book);
            }
            return books;
        }
        private static BookObservable SingleBook(JsonObject obj, int lib_id, int category_id, string category_name)
        {
            BookObservable book = new BookObservable();
            book.Category_id = category_id;
            book.Library_id = lib_id;
            book.Category_name = category_name;
            foreach (var key in obj.Keys)
            {
                IJsonValue val;
                if (!obj.TryGetValue(key, out val))
                    continue;
                switch (key)
                {
                    case "id":
                        book.book_id = (int)val.GetNumber();
                        break;
                    case "title":
                        book.book_title = val.GetString();
                        break;
                    case "author":
                        book.book_author = val.GetString();
                        break;
                    case "description":
                        book.book_description = val.GetString();
                        break;
                    case "thumb_url":
                        book.thumb_url = val.GetString();
                        break;
                    case "file_url":
                        book.file_url = val.GetString();
                        break;
                    case "file_size":
                        book.file_size = (int)val.GetNumber();
                        break;
                }
            }
            return book;
        }
        public static LibraryObservable GetLibrary(JsonArray LibraryArray, int id)
        {
            LibraryObservable library = new LibraryObservable();
            var CategoryList = (from i in LibraryArray select i.GetObject()).ToList();
            library.library_id = id;
            library.categories = GetLibraryCategories(CategoryList, id);
            return library;
        }
        public static async Task<LibraryObservable> Current_Library(string username, string password, int school_id)
        {
            LibraryObservable Current = new LibraryObservable();
            try
            {
                var library_httpclient = new HttpClient();
                var library_postData = new List<KeyValuePair<string, string>>();
                library_postData.Add(new KeyValuePair<string, string>("email", username));
                library_postData.Add(new KeyValuePair<string, string>("password", password));
                library_postData.Add(new KeyValuePair<string, string>("id", school_id.ToString()));
                var library_formContent = new FormUrlEncodedContent(library_postData);
                var library_response = await library_httpclient.PostAsync("http://eshuli.rw/liveapis/books.json", library_formContent);
                var library_result = await library_response.Content.ReadAsStreamAsync();
                var library_streamReader = new System.IO.StreamReader(library_result);
                var library_responseContent = library_streamReader.ReadToEnd().Trim().ToString();
                var library = JsonArray.Parse(library_responseContent);
                Current = GetLibrary(library, school_id);
            }
            catch 
            {
                
            }
            return Current;
        }     
        #endregion
    }
}
