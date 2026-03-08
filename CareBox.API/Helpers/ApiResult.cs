namespace CareBox.API.Helpers
{
    public class ApiResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; } // هنا نستخدم object ليقبل أي نوع بيانات
    }
}
