using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using FaceAuth.Model;
using FaceAuth.Properties;
using FaceAuth.Util;

namespace FaceAuth.ViewModel
{
    class RegisterViewModel : BaseViewModel
    {
        private string _name;
        private string _email;
        private ObservableCollection<ImageItemViewModel> _imageItems;




        public ICommand BackCommand { get; private set; }
        public ICommand RegisterCommand { get; private set; }
        public ICommand AddImageCommand { get; private set; }

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Email
        {
            get
            {
                return _email;
            }

            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public ObservableCollection<ImageItemViewModel> ImageItems
        {
            get
            {
                return _imageItems;
            }

            set
            {
                _imageItems = value;
                OnPropertyChanged(nameof(ImageItems));
            }
        }

        public override void Init()
        {
            //configure window
            WindowProperties.Height = 500;
            WindowProperties.Width = 400;
            WindowProperties.CanResize = false;
            WindowProperties.Title = "Create Account...";
        }

        public RegisterViewModel()
        {
            ImageItems = new ObservableCollection<ImageItemViewModel>();
            BackCommand = new RelayCommand((p) =>
            {
                /*go back*/
                NavigationController.Instance.ShowView<LoginViewModel>("start");
            });

            RegisterCommand = new RelayCommand(async p =>
            {
                var images = ImageItems.Select(i => ImageUtil.BytesFromBitmap(i.Image));
                var user = new User { email = Email, name = Name };
                var loadView = NavigationController.Instance.ShowView<LoadingViewModel>("load");
                loadView.Message = Resources.Registering;

                var success = await BackendConnector.Instance.TryRegister(user, images);

                var msg = NavigationController.Instance.ShowView<MessageViewModel>("register-result");
                msg.ButtonMessage = Resources.Ok;
                msg.Message = success ? "Account created.\nYou can login now" : "Error!\nRegistration failed";
                msg.MessageActionCommand = new RelayCommand(o =>
                {
                    NavigationController.Instance.DeleteView<RegisterViewModel>("register");
                    NavigationController.Instance.ShowView<LoginViewModel>("start");
                });


            }, p => DataValid());   //only allow registration when valid file is provided


            AddImageCommand = new RelayCommand((p) =>
            {
                //show face capture view to tace face snapshot
                var view = NavigationController.Instance.ShowView<FaceCaptureViewModel>("capture");

                //when image is provided, add it to the ImageItems property. this must happen in the ui context
                view.OnSnapshot += (s, img) =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var imgSource = ImageUtil.ImageSourceFromBitmap(img);
                        var item = new ImageItemViewModel() { ImageSource = imgSource, IsAddButton = false, Image = img };

                        //when remove button is clicked, remove the item from the ImageItems property
                        item.OnImageRemove += (sndr, eargs) =>
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                ImageItems.Remove(item);
                            });
                        };
                        ImageItems.Add(item);

                        //switch back to the register overview and destroy the capture view to free resources
                        NavigationController.Instance.ShowView<RegisterViewModel>("register");
                        NavigationController.Instance.DeleteView<FaceCaptureViewModel>("capture");
                    });
                };
            }, p => ImageItems.Count < 6);  //maximum images are 6
        }

        /// <summary>
        /// Registration file is only valid when name is given and consists only aus alphabecical characters,
        /// email maches regex i googled and at least one face snapshot is provided
        /// TODO: check if username is free, no account with same email exists and image has good quality (use faceAPI)
        /// </summary>
        /// <returns></returns>
        private bool DataValid()
        {
            var nameValid = Name != null && Regex.IsMatch(Name, @"^[a-zA-Z]+$");
            var emailValid = Email != null && Regex.IsMatch(Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            var imagesAdded = ImageItems?.Count != 0;

            return nameValid && emailValid && imagesAdded;
        }
    }
}