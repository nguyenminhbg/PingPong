using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sport.News
{
    // Dữ liệu trả về chung với mỗi query
    public abstract class MsgResponse
    {
        public bool success;        // Thành công hay thất bại
        public string message;      // Nội dung thông tin thêm của Message
        public int code;            // Mã http code trả về tương ứng
    }

    // Định danh mục dùng chung trong hệ thống
    public class EntityID
    {
        public int id { set; get; }          // ID của mục
        public string name { set; get; }     // Tên mục
        public string slug { set; get; }     // Tên mục không tiếng việt sử dụng trong query
        public string title{  set; get; }    // Tên tiêu đề nếu có
    }

    // Bản tin login
    public class LoginResponse : MsgResponse
    {
        public class Data
        {
            public string auth_token;
        }
        public Data data;
    }

    // Dữ liệu chung trả về khi query data
    public abstract class DataResponse
    {
        public int page;        // Trang hiện tại
        public int current;     // Số bản thi trong trang hiện tại
        public int pageCount;   // Tổng số trang hiện có
    }

    // Dữ liệu trả về theo từng Tag 
    public class Tag
    {
        public int id;          // ID của tag
        public string name;     // Tên tag utf-8
        public string slug;     // Tên tag không tiếng việt sử dụng trong query
    }

    // Dữ liệu về theo từng Category
    public class Category
    {
        public int id;          // ID của Category
        public string name;     // Tên tag utf-8
        public string slug;     // Tên tag không tiếng việt sử dụng trong query
    }

    // Dữ liệu về theo từng giải đấu
    public class Tournament
    {
        public int id;          // ID của Tournament
        public string name;     // Tên tag utf-8
        public string slug;     // Tên tag không tiếng việt sử dụng trong query
    }

    // Dữ liệu trả về khi lấy thông tin theo tag
    public class ListTagsResponse : MsgResponse
    {
        public class Data : DataResponse
        {
            public List<EntityID> tags;
        }
        public Data data;
    }

    // Dữ liệu trả về theo từng Category
    public class ListCategoriesResponse : MsgResponse
    {
        public class Data : DataResponse
        {
            public List<EntityID> categories;
        }
        public Data data;
    }

    // Dữ liệu trả về khi quey Tournament
    public class ListTournamentsResponse : MsgResponse
    {
        public class Data : DataResponse
        {
            public List<EntityID> tournaments;
        }
        public Data data;
    }

    // Tin tức theo giải đấu
    public class News : EntityID
    {
        public string description { set; get; }  // 
        public DateTime publish_date { set; get; } // Ngày đăng tin
        public class User
        {
            public string username { set; get; }
        }
        public User user { set; get; }           // Tên user đăng tin
        public int view_number { set; get; }     // Lượt xem
        public string image { set; get; }        // Ảnh cover đại diện
        public DateTime create_at { set; get; }  // Ngày giờ tạo tin tức
        public DateTime updated_at { set; get; } // Ngày giờ cập nhật tin tức
    }

    // Chi tiết tin tức
    public class NewsDetail : News
    {
        public string content;               // Nội dung bài viết
        public List<EntityID> categories;    // Danh mục category liên quan
        public List<EntityID> tags;          // Danh mục tag liên quan
        public List<EntityID> related;
    }

    // Lấy về tin tức trong giải đấu
    public class ListTournamentNewsResponse : MsgResponse
    {
        public class Data
        {
            public EntityID tournament;
            public List<News> news;
        }
        public Data data;
    }

    // Lấy tin tức theo Categories
    public class ListCategoriesNewsResponse : MsgResponse
    {
        public class Data
        {
            public EntityID category;
            public List<News> news;
        }
        public Data data;
    }

    // Lấy tin tức theo Tag
    public class ListTagNewsResponse : MsgResponse
    {
        public class Data
        {
            public EntityID tag;
            public List<News> news;
        }
        public Data data;
    }

    // Lấy chi tiết tin tức
    public class NewsDetailResponse : MsgResponse
    {
        public NewsDetail data;
    }

    // Class lấy giá trị
    public static class PingPongNews
    {
        public static string username = "admin";
        public static string password = "kingexpress";

        public static LoginResponse Login()
        {
            // Tạo 1 kết nối tới Server với Url=https://sporttv.vn/api/news/login/ và sử dụng phương thức kết nối Post
            HttpRequestMessage http_req = new HttpRequestMessage(HttpMethod.Post, "https://sporttv.vn/api/news/login/");
            //Khởi tạo nội dung content
            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new StringContent(username), "username");
            content.Add(new StringContent(password), "password");
            // Set giá trị Content cho Http
            http_req.Content = content;
            LoginResponse loginResponse = new LoginResponse { code = 500, success = false };
            try
            {
                // Gửi 1 Request lên Server
                var res = client.SendAsync(http_req, HttpCompletionOption.ResponseContentRead).Result;
                // Lấy kết quả trả về
                if (res.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    loginResponse.code = (int)res.StatusCode;
                    loginResponse.message = res.ReasonPhrase;
                    loginResponse.success = false;
                    return loginResponse;
                }
                // Covert kết quả trả về từ Json sang Object
                loginResponse = JsonConvert.DeserializeObject<LoginResponse>(res.Content.ReadAsStringAsync().Result);
                // Set giá trị xác thực từ Server trả về
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResponse.data.auth_token);
                // Thêm giá trị Header vào httpClient
                client.DefaultRequestHeaders.Add("Content-Type", "application/json;charset=utf-8");
            }
            catch (OperationCanceledException e)
            {
                loginResponse.message = e.Message;
            }
            catch (Exception e)
            {
                loginResponse.message = e.Message;
            }
            return loginResponse;
        }

        public static ListTagsResponse GetListTags(int pageNumber, int limit)
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, "https://sporttv.vn/api/news/tag");
            req.Content = new FormUrlEncodedContent(new Dictionary<string, string> { { "page", pageNumber.ToString() }, { "limit", limit.ToString() } });
            ListTagsResponse listTagsResponse = new ListTagsResponse() { code = 500, success = false };

            try
            {
                HttpResponseMessage res = client.SendAsync(req, HttpCompletionOption.ResponseContentRead).Result;
                if (res.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    listTagsResponse.code = (int)res.StatusCode;
                    listTagsResponse.message = res.ReasonPhrase;
                    listTagsResponse.success = false;
                    return listTagsResponse;
                }
                listTagsResponse = JsonConvert.DeserializeObject<ListTagsResponse>(res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception e)
            {
                listTagsResponse.message = e.Message;
            }
            return listTagsResponse;
        }

        public static async Task<ListCategoriesResponse> GetListCategories(int pageNumber, int limit)
        {
            
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, "https://sporttv.vn/api/news/category");
            req.Content = new FormUrlEncodedContent(new Dictionary<string, string> { { "page", pageNumber.ToString() }, { "limit", limit.ToString() } });
            ListCategoriesResponse listCategoriesResponse = new ListCategoriesResponse() { code = 500, success = false };
            try
            {
                HttpResponseMessage res =await  client.SendAsync(req, HttpCompletionOption.ResponseContentRead);
                if (res.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    listCategoriesResponse.code = (int)res.StatusCode;
                    listCategoriesResponse.message = res.ReasonPhrase;
                    listCategoriesResponse.success = false;
                    
                }
                listCategoriesResponse = JsonConvert.DeserializeObject<ListCategoriesResponse>(res.Content.ReadAsStringAsync().Result);
            }
            catch (OperationCanceledException e)
            {
                listCategoriesResponse.message = e.Message;
            }
            catch (Exception e)
            {
                listCategoriesResponse.message = e.Message;
            }

            return await Task.FromResult<ListCategoriesResponse>(listCategoriesResponse);
        }

        public static ListTournamentsResponse GetListTournaments(int pageNumber, int limit)
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, "https://sporttv.vn/api/news/tournament");
            req.Content = new FormUrlEncodedContent(new Dictionary<string, string> { { "page", pageNumber.ToString() }, { "limit", limit.ToString() } });
            ListTournamentsResponse listTournamentsResponse = new ListTournamentsResponse() { code = 500, success = false };
            try
            {
                HttpResponseMessage res = client.SendAsync(req, HttpCompletionOption.ResponseContentRead).Result;
                if (res.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    listTournamentsResponse.code = (int)res.StatusCode;
                    listTournamentsResponse.message = res.ReasonPhrase;
                    listTournamentsResponse.success = false;
                    return listTournamentsResponse;
                }
                listTournamentsResponse = JsonConvert.DeserializeObject<ListTournamentsResponse>(res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception e)
            {
                listTournamentsResponse.message = e.Message;
            }
            return listTournamentsResponse;
        }

        // Param slug là tên ngắn gọi của giải đấu được trả về trong ListTournamentsResponse.data.tournaments[i].slug
        public static ListTournamentNewsResponse GetTournamentNews(int pageNumber, int limit, string slug)
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, string.Format("https://sporttv.vn/api/news/tournament/{0}", slug));
            req.Content = new FormUrlEncodedContent(new Dictionary<string, string> { { "page", pageNumber.ToString() }, { "limit", limit.ToString() } });
            ListTournamentNewsResponse listTournamentNewsResponse = new ListTournamentNewsResponse() { code = 500, success = false };

            try
            {
                HttpResponseMessage res = client.SendAsync(req, HttpCompletionOption.ResponseContentRead).Result;
                if (res.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    listTournamentNewsResponse.code = (int)res.StatusCode;
                    listTournamentNewsResponse.message = res.ReasonPhrase;
                    listTournamentNewsResponse.success = false;
                    return listTournamentNewsResponse;
                }
                listTournamentNewsResponse = JsonConvert.DeserializeObject<ListTournamentNewsResponse>(res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception e)
            {
                listTournamentNewsResponse.message = e.Message;
            }
            return listTournamentNewsResponse;
        }

        // Lấy tin tức cụ thể theo Category, slug là tên được trả về trong ListCategoriesResponse.data.catetories[i].slug
        public static async Task<ListCategoriesNewsResponse> GetCategoryNews(int pageNumber, int limit, string slug)
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, string.Format("https://sporttv.vn/api/news/category/{0}", slug));
            req.Content = new FormUrlEncodedContent(new Dictionary<string, string> { { "page", pageNumber.ToString() }, { "limit", limit.ToString() } });
            ListCategoriesNewsResponse listCategoriesNewsResponse = new ListCategoriesNewsResponse() { code = 500, success = false };
            try
            {
                HttpResponseMessage res =await client.SendAsync(req, HttpCompletionOption.ResponseContentRead);
                if (res.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    listCategoriesNewsResponse.code = (int)res.StatusCode;
                    listCategoriesNewsResponse.message = res.ReasonPhrase;
                    listCategoriesNewsResponse.success = false;
                    return listCategoriesNewsResponse;
                }
                listCategoriesNewsResponse = JsonConvert.DeserializeObject<ListCategoriesNewsResponse>(res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception e)
            {
                listCategoriesNewsResponse.message = e.Message;
            }
            return await Task.FromResult<ListCategoriesNewsResponse>(listCategoriesNewsResponse);
        }

        // Lấy tin tức cụ thể theo Tags, slug là tên được trả về trong ListTagsResponse.data.tags[i].slug
        public static ListTagNewsResponse GetTagNews(int pageNumber, int limit, string slug)
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, string.Format("https://sporttv.vn/api/news/tag/{0}", slug));
            req.Content = new FormUrlEncodedContent(new Dictionary<string, string> { { "page", pageNumber.ToString() }, { "limit", limit.ToString() } });
            ListTagNewsResponse listTagNewsResponse = new ListTagNewsResponse() { code = 500, success = false };
            try
            {
                HttpResponseMessage res = client.SendAsync(req, HttpCompletionOption.ResponseContentRead).Result;
                if (res.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    listTagNewsResponse.code = (int)res.StatusCode;
                    listTagNewsResponse.message = res.ReasonPhrase;
                    listTagNewsResponse.success = false;
                    return listTagNewsResponse;
                }
                listTagNewsResponse = JsonConvert.DeserializeObject<ListTagNewsResponse>(res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception e)
            {
                listTagNewsResponse.message = e.Message;
            }
            return listTagNewsResponse;
        }

        // Lấy ra chi tiết tin tức cụ thể, slug là giá trị trả về từ danh sách tin tức
        public static NewsDetailResponse GetNewsDetail(string slug)
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, string.Format("https://sporttv.vn/api/news/{0}", slug));
            NewsDetailResponse newsDetailResponse = new NewsDetailResponse() { code = 500, success = false };

            try
            {
                HttpResponseMessage res = client.SendAsync(req, HttpCompletionOption.ResponseContentRead).Result;

                if (res.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    newsDetailResponse.code = (int)res.StatusCode;
                    newsDetailResponse.message = res.ReasonPhrase;
                    newsDetailResponse.success = false;
                    return newsDetailResponse;
                }
                newsDetailResponse = JsonConvert.DeserializeObject<NewsDetailResponse>(res.Content.ReadAsStringAsync().Result);
            }
            catch (Exception e)
            {
                newsDetailResponse.message = e.Message;
            }
            return newsDetailResponse;
        }

        private static HttpClient client = new HttpClient();
    }
}
