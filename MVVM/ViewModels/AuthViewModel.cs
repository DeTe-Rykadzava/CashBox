﻿using Cashbox.Core.Commands;
using Cashbox.Service;
using System.Windows;
using System.ComponentModel.DataAnnotations;
using Cashbox.Core;
using Cashbox.MVVM.Models;
using Cashbox.MVVM.ViewModels.Data;

namespace Cashbox.MVVM.ViewModels
{
    public class AuthViewModel : ViewModelBase
    {
        #region Props

        #region SwipeAuthMethod
        private Visibility _authMethodPinVisibility = Visibility.Visible;
        public Visibility AuthMethodPinVisibility
        {
            get => _authMethodPinVisibility;
            set => Set(ref _authMethodPinVisibility, value);
        }

        private Visibility _authMethodLPVisibility = Visibility.Collapsed;
        public Visibility AuthMethodLPVisibility
        {
            get => _authMethodLPVisibility;
            set => Set(ref _authMethodLPVisibility, value);
        }

        private bool _authMethodPinEnabled = true;
        public bool IsAuthMethodPinEnabled
        {
            get => _authMethodPinEnabled;
            set => Set(ref _authMethodPinEnabled, value);
        }

        private bool _authMethodLPEnabled = false;
        public bool IsAuthMethodLPEnabled
        {
            get => _authMethodLPEnabled;
            set => Set(ref _authMethodLPEnabled, value);
        }

        #endregion

        #region UserData

        private string _login = string.Empty;
        [Required(AllowEmptyStrings = false)]
        public string Login
        {
            get => _login;
            set => Set(ref _login, value);
        }

        private string _password = string.Empty;
        [Required(AllowEmptyStrings = false)]
        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        private int _pin;
        [Required(AllowEmptyStrings = false)]
        public int Pin
        {
            get => _pin;
            set => Set(ref _pin, value);
        }

        private bool _tfa = false;
        [Required(AllowEmptyStrings = false)]
        public bool TFA
        {
            get => _tfa;
            set => Set(ref _tfa, value);
        }

        #endregion

        #endregion


        #region Commands

        public RelayCommand SwipeAuthMethodVisibilityCommand { get; set; }
        private bool CanSwipeAuthMethodVisibilityCommandExecute(object p) => true;
        private void OnSwipeAuthMethodVisibilityCommandExecuted(object p)
        {
            switch (IsAuthMethodPinEnabled)
            {
                case true:
                    AuthMethodPinVisibility = Visibility.Visible;
                    AuthMethodLPVisibility = Visibility.Collapsed;
                    break;
                case false:
                    AuthMethodPinVisibility = Visibility.Collapsed;
                    AuthMethodLPVisibility = Visibility.Visible;
                    break;
            }
        }

        public RelayCommand AuthByLogPassCommand { get; set; }
        private bool CanAuthByLogPassCommandExecute(object p) => true;
        private async void OnAuthByLogPassCommandExecuted(object p)
        {
            UserViewModel? user = await UserViewModel.GetUserByLogPass(Login, Password);
            if (user == null) { MessageBox.Show("lox"); return; }
            MessageBox.Show(user.UserInfo?.Name);
        }

        #endregion


        #region Navigation
        private INavigationService? _navigationService;
        public INavigationService? NavigationService
        {
            get => _navigationService;
            set => Set(ref _navigationService, value);
        }
        public RelayCommand NavigateMainPageCommand { get; set; }
        private bool CanNavigateMainPageCommandExecute(object p) => true;
        private void OnNavigateMainPageCommandExecuted(object p)
        {
            NavigationService?.NavigateTo<LoadingViewModel>();
        }
        #endregion

        public AuthViewModel(INavigationService? navService)
        {
            NavigationService = navService;
            NavigateMainPageCommand = new RelayCommand(OnNavigateMainPageCommandExecuted, CanNavigateMainPageCommandExecute);
            SwipeAuthMethodVisibilityCommand = new RelayCommand(OnSwipeAuthMethodVisibilityCommandExecuted, CanSwipeAuthMethodVisibilityCommandExecute);
            AuthByLogPassCommand = new RelayCommand(OnAuthByLogPassCommandExecuted, CanAuthByLogPassCommandExecute);
        }
    }
}
