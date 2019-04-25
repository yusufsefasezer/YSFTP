﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace YSFTP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            string currentCulture = Thread.CurrentThread.CurrentCulture.Name;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(currentCulture);
            base.OnStartup(e);
        }
    }
}
