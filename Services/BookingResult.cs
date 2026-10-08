using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookingEngine.Services
{
    public class BookingResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }

        public static BookingResult<T> Ok(T data) => new() { Success = true, Data = data };
        public static BookingResult<T> Fail(string error) => new() { Success = false, ErrorMessage = error };    
    }
}
