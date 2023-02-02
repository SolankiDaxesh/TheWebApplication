using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using TheWebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;

namespace TheWebApplication.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly IConfiguration Configuration;
        public string value = "";
        public RegistrationController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        //[HttpGet]
        //public IActionResult GetClientInfo()
        //{
        //    var ipAddress = HttpContext.Connection.RemoteIpAddress;
        //    //var macAddress = HttpContext.Connection.RemoteMacAddress;
        //    Index(ipAddress, macAddress);
        //    return Ok(new { IP = ipAddress, MAC = macAddress });
        //}
        [HttpPost]
        public IActionResult Index(RegistrationModel registration)
        {
            string externalIP = string.Empty;
            WebClient web = new();
            externalIP = web.DownloadString("http://checkip.amazonaws.com/");
            externalIP = Regex.Replace(externalIP, "<.*?>", string.Empty);
            externalIP = Regex.Replace(externalIP, "[^0-9.]", "");
            Response.WriteAsJsonAsync(new { externalIP, value });
            try
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                string connString = this.Configuration.GetConnectionString("DBConnection");
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                //string con = configuration.GetConnectionString("DBConnection");

                using (SqlConnection connection = new SqlConnection(connString))
                {
                    using (SqlCommand cmd = new SqlCommand("USP_AccountManagment", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@Action", 1);
                        cmd.Parameters.AddWithValue("@userName", registration.Name);
                        cmd.Parameters.AddWithValue("@Email", registration.Email);
                        cmd.Parameters.AddWithValue("@mobileNo", registration.PhoneNumber);
                        cmd.Parameters.AddWithValue("@password", registration.Password);
                        cmd.Parameters.AddWithValue("@cPassword", registration.CPassword);
                        connection.Open();
                        ViewData["result"] = cmd.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return View();
        }
    }
}
