﻿using BusinessLogicModule.Services;
using Ninject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows;
using UIModule.Utils;
using UIModule.ViewModels;
using UIModule.Views;

namespace UIModule
{
    public partial class App : Application
    {
        private static List<CultureInfo> m_Languages = new List<CultureInfo>();

        public static List<CultureInfo> Languages
        {
            get
            {
                return m_Languages;
            }
        }

        public DisplayRootRegistry displayRootRegistry = new DisplayRootRegistry();        

        public App()
        {
            displayRootRegistry.RegisterWindowType<AuthorizationWindowViewModel, AuthorizationWindow>();
            displayRootRegistry.RegisterWindowType<MainWindowViewModel, MainWindow>();

            InitializeComponent();
            App.LanguageChanged += App_LanguageChanged;

            m_Languages.Clear();
            m_Languages.Add(new CultureInfo("en-US")); //Нейтральная культура для этого проекта
            m_Languages.Add(new CultureInfo("ru-RU"));

            Language = UIModule.Properties.Settings.Default.DefaultLanguage;
        }


        private IKernel _iocKernel;

        protected override void OnStartup(StartupEventArgs e)
        {
            IocKernel.Initialize(new IocConfiguration());

            base.OnStartup(e);

            _iocKernel = new StandardKernel();

            Current.MainWindow = _iocKernel.Get<AuthorizationWindow>();
            Current.MainWindow.Show();
        }

        //Евент для оповещения всех окон приложения
        public static event EventHandler LanguageChanged;

        public static CultureInfo Language
        {
            get
            {
                return System.Threading.Thread.CurrentThread.CurrentUICulture;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                if (value == System.Threading.Thread.CurrentThread.CurrentUICulture) return;

                //Меняем язык приложения:
                System.Threading.Thread.CurrentThread.CurrentUICulture = value;

                //Создаём ResourceDictionary для новой культуры
                ResourceDictionary dict = new ResourceDictionary();
                switch (value.Name)
                {
                    case "ru-RU":
                        dict.Source = new Uri(String.Format("Resources/lang.{0}.xaml", value.Name), UriKind.Relative);
                        break;
                    default:
                        dict.Source = new Uri("Resources/lang.xaml", UriKind.Relative);
                        break;
                }

                //Находим старую ResourceDictionary и удаляем его и добавляем новую ResourceDictionary
                ResourceDictionary oldDict = (from d in Application.Current.Resources.MergedDictionaries
                                              where d.Source != null && d.Source.OriginalString.StartsWith("Resources/lang.")
                                              select d).First();
                if (oldDict != null)
                {
                    int ind = Application.Current.Resources.MergedDictionaries.IndexOf(oldDict);
                    Application.Current.Resources.MergedDictionaries.Remove(oldDict);
                    Application.Current.Resources.MergedDictionaries.Insert(ind, dict);
                }
                else
                {
                    Application.Current.Resources.MergedDictionaries.Add(dict);
                }

                //Вызываем евент для оповещения всех окон.
                LanguageChanged(Application.Current, new EventArgs());
            }
        }

        private void App_LanguageChanged(Object sender, EventArgs e)
        {
            UIModule.Properties.Settings.Default.DefaultLanguage = Language;
            UIModule.Properties.Settings.Default.Save();
        }
    }
}
