using Db.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qualco3.Common
{
    
    /// <summary>
    /// This is where you customize the navigation sidebar
    /// </summary>
    public static class ModuleHelper
    {
        public enum Module
        {
            Home,
            Bills,
            Settlements,
            ApplicationUsers,
            GetFile,
            PostFile,
            Error,
            Contact
        }

        public static SidebarMenu AddHeader(string name)
        {
            return new SidebarMenu
            {
                Type = SidebarMenuType.Header,
                Name = name,
            };
        }

        public static SidebarMenu AddTree(string name, string iconClassName = "fa fa-link")
        {
            return new SidebarMenu
            {
                Type = SidebarMenuType.Tree,
                IsActive = false,
                Name = name,
                IconClassName = iconClassName,
                URLPath = "#",
            };
        }

        public static SidebarMenu AddModule(Module module, Tuple<int, int, int> counter = null)
        {
            if (counter == null)
                counter = Tuple.Create(0, 0, 0);

            switch (module)
            {
                case Module.Home:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Home",
                        IconClassName = "fa fa-link",
                        URLPath = "/Home/Index",
                        LinkCounter = counter,
                    };
                case Module.Bills:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Manage Bills",
                        IconClassName = "fa fa-sign-in",
                        URLPath = "/Bills/Index",
                        LinkCounter = counter,
                    };
                case Module.Settlements:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Settlements",
                        IconClassName = "fa fa-user-plus",
                        URLPath = "/Settlements/Index",
                        LinkCounter = counter,
                    };
                case Module.ApplicationUsers:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Manage Users",
                        IconClassName = "fa fa-group",
                        URLPath = "/ApplicationUsers/Index",
                        LinkCounter = counter,
                    };
               case Module.GetFile:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Get File",
                        IconClassName = "fa fa-group",
                        URLPath = "/CitizenDepts/GetFile",
                        LinkCounter = counter,
                    };
                case Module.PostFile:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Post File",
                        IconClassName = "fa fa-group",
                        URLPath = "/PostFile/PostFile",
                        LinkCounter = counter,
                    };
                case Module.Contact:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Contact",
                        IconClassName = "fa fa-phone",
                        URLPath = "/Home/Contact",
                        LinkCounter = counter,
                    };
                case Module.Error:
                    return new SidebarMenu
                    {
                        Type = SidebarMenuType.Link,
                        Name = "Error",
                        IconClassName = "fa fa-warning",
                        URLPath = "/Home/Error",
                        LinkCounter = counter,
                    };

                default:
                    break;
            }

            return null;
        }
    }
}
