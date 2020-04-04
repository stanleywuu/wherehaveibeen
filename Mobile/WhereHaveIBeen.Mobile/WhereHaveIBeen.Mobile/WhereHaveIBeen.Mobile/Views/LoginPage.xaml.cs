using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhereHaveIBeen.Mobile.ApiClient;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WhereHaveIBeen.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class LoginPage : ContentPage
    {
        Entry usernameEntry, passwordEntry;
        Label messageLabel;

        public class User
        {
            public string Username { get; set; }

            public string Password { get; set; }

            public string Email { get; set; }
        }

        public LoginPage()
        {
            var toolbarItem = new ToolbarItem
            {
                Text = "Sign Up"
            };
            toolbarItem.Clicked += OnSignUpButtonClicked;
            ToolbarItems.Add(toolbarItem);

            messageLabel = new Label();
            usernameEntry = new Entry
            {
                Placeholder = "username"
            };
            passwordEntry = new Entry
            {
                IsPassword = true
            };
            var loginButton = new Button
            {
                Text = "Login"
            };
            loginButton.Clicked += OnLoginButtonClicked;

            Title = "Login";
            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Children = {
                    new Label { Text = "Username" },
                    usernameEntry,
                    new Label { Text = "Password" },
                    passwordEntry,
                    loginButton,
                    messageLabel
                }
            };
        }

        async void OnSignUpButtonClicked(object sender, EventArgs e)
        {
            await ApiCalls.Login(usernameEntry.Text?.ToLowerInvariant(), passwordEntry.Text);
        }

        async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(usernameEntry.Text) || string.IsNullOrEmpty(passwordEntry.Text))
            {
                messageLabel.Text = "Please enter a user name and a password";
            }

            var response = await ApiCalls.Login(usernameEntry.Text?.ToLowerInvariant(), passwordEntry.Text);

            var isValid = response.StatusCode == System.Net.HttpStatusCode.OK;

            if (isValid)
            {
                App.PersistUser(response.Data);
                Navigation.InsertPageBefore(new MapPage(), this);
                await Navigation.PopAsync();
            }
            else
            {
                messageLabel.Text = "Login failed";
                passwordEntry.Text = string.Empty;
            }
        }

    }
}