using System.Text.Json;

namespace HSU.PTWeb.AnhPH.BookStore.Helpers
{
    // Helper để lưu/đọc giỏ hàng trong Session
    public static class SessionHelper
    {
        public static void SetObject<T>(ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T GetObject<T>(ISession session, string key)
        {
            var data = session.GetString(key);
            return data == null ? default(T) : JsonSerializer.Deserialize<T>(data);
        }

        // Alias for compatibility
        public static T GetObjectFromJson<T>(ISession session, string key)
        {
            return GetObject<T>(session, key);
        }
    }
}
