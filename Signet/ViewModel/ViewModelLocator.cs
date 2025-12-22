/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Signet"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using CommonServiceLocator;

namespace Signet.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}
            SimpleIoc.Default.Register<Login_Window_ViewModel>();
            SimpleIoc.Default.Register<Home_ViewModel>();
            SimpleIoc.Default.Register<Login_Window_ViewModel>();
            SimpleIoc.Default.Register<Setting_ViewModel>();
            SimpleIoc.Default.Register<About_ViewModel>();
            SimpleIoc.Default.Register<Log_ViewModel>();
            SimpleIoc.Default.Register<Users_ViewModel>();
            SimpleIoc.Default.Register<AuthorityCfg_ViewModel>();


        }


        public Login_Window_ViewModel Login
        {
            get
            {
                return ServiceLocator.Current.GetInstance<Login_Window_ViewModel>();
            }
        }

        public Home_ViewModel Home
        {
            get
            {
                return ServiceLocator.Current.GetInstance<Home_ViewModel>();
            }
        }

        public Users_ViewModel Users
        {
            get
            {
                return ServiceLocator.Current.GetInstance<Users_ViewModel>();
            }
        }

        //public Login_Window_ViewModel Login
        //{
        //    get
        //    {
        //        return ServiceLocator.Current.GetInstance<Login_Window_ViewModel>();
        //    }
        //}

        public Setting_ViewModel Setting
        {
            get
            {
                return ServiceLocator.Current.GetInstance<Setting_ViewModel>();
            }
        }

        public About_ViewModel About
        {
            get
            {
                return ServiceLocator.Current.GetInstance<About_ViewModel>();
            }
        }

        public Log_ViewModel Log
        {
            get
            {
                return ServiceLocator.Current.GetInstance<Log_ViewModel>();
            }
        }

        public AuthorityCfg_ViewModel AuthorityCfg
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AuthorityCfg_ViewModel>();
            }
        }
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }

    }
}