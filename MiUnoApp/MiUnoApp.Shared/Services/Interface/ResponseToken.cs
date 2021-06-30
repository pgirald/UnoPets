using MiUnoApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiUnoApp.Services.Interface
{
    public class ResponseToken
    {
        public string Token { get; set; }

        public DateTimeOffset Expiration { get; set; }

        public User User { get; set; }
    }
}
