﻿namespace ProductsTest.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using System.ComponentModel;
    using System.Windows.Input;
    using Services;
    using Xamarin.Forms;

    public class LoginViewModel : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Services
        ApiService apiService;
        DataService dataService;
        DialogService dialogService;
        NavigationService navigationService;
        #endregion

        #region Attributes
        string _email;
        string _password;
        bool _isToggled;
        bool _isRunning;
        bool _isEnabled;
        #endregion

        #region Properties
        public bool IsEnabled
        {
            get {
                return _isEnabled;
            }
            set {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(IsEnabled)));
                }
            }
        }

        public bool IsRunning
        {
            get {
                return _isRunning;
            }
            set {
                if (_isRunning != value)
                {
                    _isRunning = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(IsRunning)));
                }
            }
        }

        public bool IsToggled
        {
            get {
                return _isToggled;
            }
            set {
                if (_isToggled != value)
                {
                    _isToggled = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(IsToggled)));
                }
            }
        }

        public string Password
        {
            get {
                return _password;
            }
            set {
                if (_password != value)
                {
                    _password = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(Password)));
                }
            }
        }

        public string Email
        {
            get {
                return _email;
            }
            set {
                if (_email != value)
                {
                    _email = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(Email)));
                }
            }
        }
        #endregion

        #region Constructors
        public LoginViewModel()
        {
            apiService = new ApiService();
            dataService = new DataService();
            dialogService = new DialogService();
            navigationService = new NavigationService();

            IsEnabled = true;
            IsToggled = true;
        }
        #endregion

        #region Commands
        public ICommand RecoverPasswordCommand
        {
            get {
                return new RelayCommand(RecoverPassword);
            }
        }

        async void RecoverPassword()
        {
            MainViewModel.GetInstance().PasswordRecovery =
                new PasswordRecoveryViewModel();
            await navigationService.NavigateOnLogin("PasswordRecoveryView");
        }


        public ICommand LoginWithFacebookCommand
        {
            get {
                return new RelayCommand(LoginWithFacebook);
            }
        }

        async void LoginWithFacebook()
        {
            await navigationService.NavigateOnLogin("LoginFacebookView");
        }

        public ICommand RegisterNewUserCommand
        {
            get {
                return new RelayCommand(RegisterNewUser);
            }
        }

        async void RegisterNewUser()
        {
            MainViewModel.GetInstance().NewCustomer = new NewCustomerViewModel();
            await navigationService.NavigateOnLogin("NewCustomerView");
        }


        public ICommand LoginCommand
        {
            get {
                return new RelayCommand(Login);
            }
        }

        async void Login()
        {
            if (string.IsNullOrEmpty(Email))
            {
                await dialogService.ShowMessage(
                    "Error",
                    "You must enter an email.");
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                await dialogService.ShowMessage(
                    "Error",
                    "You must enter a password.");
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await dialogService.ShowMessage("Error", connection.Message);
                return;
            }

            var urlAPI = Application.Current.Resources["URLAPI"].ToString();

            var response = await apiService.GetToken(
                urlAPI,
                Email,
                Password);

            if (response == null)
            {
                IsRunning = false;
                IsEnabled = true;
                await dialogService.ShowMessage(
                    "Error",
                    "The service is not available, please try latter.");
                Password = null;
                return;
            }

            if (string.IsNullOrEmpty(response.AccessToken))
            {
                IsRunning = false;
                IsEnabled = true;
                await dialogService.ShowMessage(
                    "Error",
                    response.ErrorDescription);
                Password = null;
                return;
            }

            response.IsRemembered = IsToggled;
            response.Password = Password;
            dataService.DeleteAllAndInsert(response);

            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.Token = response;
            mainViewModel.RegisterDevice();
            mainViewModel.Categories = new CategoriesViewModel();
            navigationService.SetMainPage("MasterView");

            Email = null;
            Password = null;

            IsRunning = false;
            IsEnabled = true;
        }
        #endregion
    }

    //#region Atributos
    //string _email;
    //string _password;
    //bool _isToggled;
    //bool _isRunning; //los boobleanos siempre inicializan en falso
    //bool _isEnabled;
    //#endregion

    //#region Comandos
    //public ICommand LoginCommand
    //{
    //    get { return new RelayCommand(Login); }

    //}

    //async void Login()
    //{
    //    if (string.IsNullOrEmpty(Email))
    //    {
    //        await dialogService.ShowMessage(
    //            "Error",
    //            "You must enter an email");
    //        return;
    //    }

    //    if (string.IsNullOrEmpty(Password))
    //    {
    //        await dialogService.ShowMessage(
    //            "Error",
    //            "You must enter a password");
    //        return;
    //    }

    //    IsRunning = true;
    //    IsEnabled = false;

    //    var connection = await apiService.CheckConnection();

    //    if (!connection.IsSuccess)
    //    {
    //        await dialogService.ShowMessage("Error", connection.Message);
    //        IsRunning = false;
    //        IsEnabled = true;
    //        return;
    //    }

    //    var response = await apiService.GetToken("http://productszuluapi.azurewebsites.net", Email, Password);

    //    if (response == null)
    //    {
    //        await dialogService.ShowMessage("Error", "The servise is not available, please try later");
    //        IsRunning = false;
    //        IsEnabled = true;
    //        Password = null;
    //        return;
    //    }

    //    if (string.IsNullOrEmpty(response.AccessToken))
    //    {
    //        await dialogService.ShowMessage("Error", response.ErrorDescription);
    //        IsRunning = false;
    //        IsEnabled = true;
    //        Password = null;
    //        return;
    //    }

    //    var mainViewModel = MainViewModel.GetInstance();
    //    mainViewModel.Token = response;
    //    mainViewModel.Categories = new CategoriesViewModel();

    //    //una view model no debe conocer una view ni viceversa, es pecado que una viewmodel vea las views (pages)
    //    // await Application.Current.MainPage.Navigation.PushAsync(new CategoriesView());
    //    await navigationService.NavigateOnLogin("CategoriesView");
    //    Email = null;
    //    Password = null;
    //    IsRunning = false;
    //    IsEnabled = true;

    //}
    //#endregion

    //#region Constructores
    //public LoginViewModel()
    //{
    //    IsEnabled = true;
    //    IsToggled = true;
    //    dialogService = new DialogService();
    //    apiService = new ApiService();
    //    navigationService = new NavigationService();
    //}
    //#endregion

    //#region Eventos
    //public event PropertyChangedEventHandler PropertyChanged;
    //#endregion

    //#region Propiedades
    //public string Email
    //{
    //    get {
    //        return _email;
    //    }
    //    set {
    //        if (_email != value)
    //        {
    //            _email = value;
    //            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Email)));
    //        }
    //    }
    //}

    //public string Password
    //{
    //    get {
    //        return _password;
    //    }
    //    set {
    //        if (_password != value)
    //        {
    //            _password = value;
    //            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Password)));
    //        }
    //    }
    //}

    //public bool IsToggled
    //{
    //    get {
    //        return _isToggled;
    //    }
    //    set {
    //        if (_isToggled != value)
    //        {
    //            _isToggled = value;
    //            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsToggled)));
    //        }
    //    }
    //}

    //public bool IsRunning
    //{
    //    get {
    //        return _isRunning;
    //    }
    //    set {
    //        if (_isRunning != value)
    //        {
    //            _isRunning = value;
    //            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRunning)));
    //        }
    //    }
    //}

    //public bool IsEnabled
    //{
    //    get {
    //        return _isEnabled;
    //    }
    //    set {
    //        if (_isEnabled != value)
    //        {
    //            _isEnabled = value;
    //            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabled)));
    //        }
    //    }
    //}
    //#endregion

    //#region Services
    //DialogService dialogService;
    //ApiService apiService;
    //NavigationService navigationService;
    //#endregion

}
 
