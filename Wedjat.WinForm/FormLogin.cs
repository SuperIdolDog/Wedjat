using AntdUI;
using Newtonsoft.Json;
using RestSharp;
using SpeechLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wedjat.Helper;
using Wedjat.Model.Config;
using Wedjat.Model.DTO;
using Wedjat.Model.Entity;


namespace Wedjat.WinForm
{
    public partial class FormLogin : Window
    {
        private readonly RestClient _restClient;
        public FormLogin()
        {
            InitializeComponent();
            _restClient = new RestClient("http://localhost:5059/");
        }

        #region 登录事件
        private async void btn_Login_Click(object sender, EventArgs e)
        {
            string workId = input_WorkId.Text.Trim();
            string password = input_Password.Text.Trim();

            if (string.IsNullOrEmpty(workId) || string.IsNullOrEmpty(password))
            {
                AntdUI.Modal.open("提示", "请输入工号和密码！", AntdUI.TType.Error);
                return;
            }

            btn_Login.Loading=true;

            try
            {
                var request = new RestRequest("api/Scada/Login", Method.Post);
                request.AddQueryParameter("workId", workId);
                request.AddQueryParameter("password", password);

                var response = await _restClient.ExecuteAsync(request);

                if (!response.IsSuccessful)
                {
                    AntdUI.Modal.open("错误", $"HTTP 错误：{response.StatusDescription}", AntdUI.TType.Error);
                    return;
                }

                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<LoginResponse>>(response.Content);

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    LoginConfig.ProductLine = select_productLine.Text;
                    LoginConfig.Name = apiResponse.Data.Name;
                    LoginConfig.WorkId = apiResponse.Data.WorkId;
                    // AntdUI.Modal.open("成功", $"登录成功，欢迎 {apiResponse.Data.Name}！", AntdUI.TType.Success);
                    string message = $"登录成功，欢迎 {apiResponse.Data.Name}！";
                    AntdUI.Notification.success(this.FindForm(), "登录", message);
                  
                    this.Hide();
                    new FormMain().Show();
                }
                else
                {
                    string err = apiResponse?.Message
                              ?? (apiResponse?.Errors?.Any() == true ? string.Join("\n", apiResponse.Errors) : "未知错误");
                   // AntdUI.Modal.open("登录失败", err, AntdUI.TType.Error);
                    AntdUI.Notification.error(this.FindForm(), "登录", err);
                }
            }

            catch (Exception ex)
            {
                AntdUI.Modal.open("异常", ex.Message, AntdUI.TType.Error);
            }
            finally
            {
                btn_Login.Loading = false;
            }
        }
        #endregion
    }
}
