﻿using System;
using Xamarin.Forms;

namespace Manifest.Login.Apple
{
    public class AppleSignInButton : ImageButton
    {
        public AppleSignInButton()
        {
            Clicked += AppleSignInButton_Clicked;
            Source = "apple_login.png";
        }

        public AppleSignInButtonStyle ButtonStyle { get; set; } = AppleSignInButtonStyle.Black;

        private void AppleSignInButton_Clicked(object sender, EventArgs e)
        {
            SignIn?.Invoke(sender, e);
            Command?.Execute(CommandParameter);
        }

        public event EventHandler SignIn;

        public void InvokeSignInEvent(object sender, EventArgs e)
            => SignIn?.Invoke(sender, e);

        public void Dispose()
            => Clicked -= AppleSignInButton_Clicked;
    }

    public enum AppleSignInButtonStyle
    {
        Black,
        White,
        WhiteOutline
    }
}
