namespace Wedjat.MiniMES
{
    public class ApiResponse<T>
    {
        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 消息提示（成功/失败信息）
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 业务数据（成功时返回，失败时为null）
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 错误详情（失败时返回，成功时为null）
        /// </summary>
        public List<string> Errors { get; set; }

        /// <summary>
        /// 响应时间戳
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>
        /// 构建成功响应
        /// </summary>
        /// <param name="data">业务数据</param>
        /// <param name="message">成功消息</param>
        /// <returns>成功响应模型</returns>
        public static ApiResponse<T> SuccessResult(T data, string message = "操作成功")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Errors = null
            };
        }

        /// <summary>
        /// 构建失败响应（单条错误）
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <returns>失败响应模型</returns>
        public static ApiResponse<T> ErrorResult(string message)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = new List<string> { message }
            };
        }

        /// <summary>
        /// 构建失败响应（多条错误）
        /// </summary>
        /// <param name="errors">错误列表</param>
        /// <param name="message">错误摘要消息</param>
        /// <returns>失败响应模型</returns>
        public static ApiResponse<T> ErrorResult(List<string> errors, string message = "操作失败")
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Errors = errors
            };
        }
    }

    /// <summary>
    /// 无数据的API响应（用于不需要返回业务数据的场景，如删除操作）
    /// </summary>
    public class ApiResponse : ApiResponse<object>
    {
        /// <summary>
        /// 构建无数据的成功响应
        /// </summary>
        public static ApiResponse SuccessResult(string message = "操作成功")
        {
            return new ApiResponse
            {
                Success = true,
                Message = message,
                Data = null
            };
        }
    }
}
