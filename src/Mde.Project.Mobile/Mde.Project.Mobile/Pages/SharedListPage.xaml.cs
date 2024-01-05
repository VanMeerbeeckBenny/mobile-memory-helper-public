using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mde.Project.Mobile.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SharedListPage : ContentPage
    {
        public SharedListPage()
        {
            InitializeComponent();
        }

        private void Switch_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var mySwitch = sender as Switch;   
            var parent = mySwitch.Parent as Grid;

            if (parent != null)
            {
                foreach (var element in parent.Children)
                {
                    if (element is CheckBox checkBox)
                    {
                        if (mySwitch.IsToggled)
                        {
                            mySwitch.ThumbColor = ColorConverters.FromHex("#80BD72");
                            checkBox.Color = ColorConverters.FromHex("#76b5c5");
                            checkBox.IsEnabled = true;
                        }
                        else
                        {
                            mySwitch.ThumbColor = ColorConverters.FromHex("#d85757");
                            checkBox.Color = ColorConverters.FromHex("#4d565b");
                            checkBox.IsEnabled = false;
                            checkBox.IsChecked = false;//logic for this is in viewmodel,
                                                       //this is for visual effect only
                        }
                    }
                }
            }
           

        }
  
    }
}