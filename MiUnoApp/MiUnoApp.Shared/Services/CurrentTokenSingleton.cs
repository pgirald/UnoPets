using MiUnoApp.Models;
using MiUnoApp.Services.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiUnoApp.Services
{
    class CurrentTokenSingleton
    {
        public static ResponseToken CurrentToken { get; set; }

        public static EventHandler<OnCurrentUserUpdatedEventArgs> OnCurrentUserUpdated;

        public static User CurrentUser
        {
            get => CurrentToken.User;
            set
            {
                CurrentToken.User = value;
                if (OnCurrentUserUpdated != null)
                {
                    OnCurrentUserUpdated(null, new OnCurrentUserUpdatedEventArgs { CurrentUser = value });
                }
            }
        }

        public class OnCurrentUserUpdatedEventArgs : EventArgs
        {
            public User CurrentUser { get; set; }
        }
    }
}
