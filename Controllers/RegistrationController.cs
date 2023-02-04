using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Session;
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
using System.Drawing;
using NuGet.Protocol.Plugins;

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
            //Response.WriteAsJsonAsync(new { externalIP, value });
            try
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                string connString = this.Configuration.GetConnectionString("DBConnection");
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

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
                        cmd.Parameters.AddWithValue("@IpAddress", externalIP);
                        connection.Open();
                        ViewData["result"] = cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginModel login)
        {

            try
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                string connString = this.Configuration.GetConnectionString("DBConnection");
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

                SqlConnection connection = new SqlConnection(connString);
                connection.Open();
                SqlCommand cmd = new SqlCommand("USP_AccountManagment", connection);
                string status;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Action", 2);
                cmd.Parameters.AddWithValue("@userName", login.Name);
                cmd.Parameters.AddWithValue("@password", login.Password);
                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    HttpContext.Session.SetString("login", login.Name.ToString());
                    return RedirectToAction("Welcome");
                }

                if (login.Name.ToString() == null || login.Name.ToString() == "")
                {
                    ViewData["Message"] = "User Login Details Failed!!";
                }
                if (login.Name.ToString() != null)
                {
                    HttpContext.Session.SetString("login", login.Name.ToString());
                    status = "1";
                }
                else
                {
                    status = "3";
                }
                connection.Close();
                return View();
            }

            catch (Exception ex)
            {
                throw ex;
            }
            return View();
        }
        [HttpGet]
        public ActionResult Welcome()
        {
            return View();
        }
    }
}
