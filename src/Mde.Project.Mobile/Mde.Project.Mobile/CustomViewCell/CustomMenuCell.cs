using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Mde.Project.Mobile.CustomViewCell
{
    public class CustomMenuCell:ViewCell
    {
        public int MyProperty { get; set; }
        public CustomMenuCell()
        {            
            Color primaryColor = ColorConverters.FromHex("#76b5c5");
     
            var titleLabel = new Label
            {
                TextColor = primaryColor,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 15
            };
            var iconLabel = new Label
            {
                Padding = new Thickness(10, 5,0,5),
                TextColor = primaryColor,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 20,
                WidthRequest = 40
            };
            var verticaLayout = new StackLayout ();
            var horizontalLayout = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Fill };

            
            titleLabel.SetBinding(Label.TextProperty, new Binding("Title"));
            iconLabel.SetBinding(Label.TextProperty, new Binding("Icon")); 
            
            horizontalLayout.Children.Add(iconLabel);
            horizontalLayout.Children.Add(titleLabel);

            verticaLayout.Children.Add(horizontalLayout);
            iconLabel.FontFamily = GetFontPathForDevice();
            
            View = horizontalLayout;
        }

        private string GetFontPathForDevice()
        {
            object fontobject;
            Application.Current.Resources.TryGetValue("FontAwesomeSolid", out fontobject);

            OnPlatform<string> onplatform = (OnPlatform<string>)fontobject;
            string fontfamely = "";
            foreach (var item in onplatform.Platforms)
            {
                if (item.Platform.Contains(DeviceInfo.Platform.ToString()))
                {
                    fontfamely = item.Value.ToString();
                }
            }
            return fontfamely;
        }
    }
}
