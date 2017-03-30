using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using FaceAuth.Model;
using FaceAuth.Properties;
using FaceAuth.Util;

namespace FaceAuth.ViewModel
{
    class ExplorerViewModel : BaseViewModel
    {
        private ObservableCollection<FileItemViewModel> _files;
        private string _userName;

        public ICommand BackCommand { get; private set; }
        public ICommand CreateFileCommand { get; private set; }
        public ICommand DeleteFileCommand { get; private set; }

        private FileItemViewModel _selectedFile;

        public ObservableCollection<FileItemViewModel> Files
        {
            get
            {
                return _files;
            }

            set
            {
                _files = value;
                //this is a bit hacky, but it works and i didn't have much time.
                //prevents multi file selection
                foreach (var fileItemViewModel in _files)
                {
                    fileItemViewModel.OnSelect += (s, e) =>
                    {
                        foreach (var file in _files.Where(f => f.IsSelected))
                        {
                            if (file != fileItemViewModel)
                                file.IsSelected = false;
                        }
                        _selectedFile = (FileItemViewModel) s;
                    };
                }
                OnPropertyChanged(nameof(Files));
            }
        }

        public override void Init()
        {
            WindowProperties.Width = 960;
            WindowProperties.Height = 720;
            WindowProperties.CanResize = true;
            _userName = ((MainViewModel)NavigationController.Instance.ViewRoot).UserName;
            WindowProperties.Title = $"{_userName}'s Files";

        }

        public override void Stop()
        {
            BackendConnector.Instance.OnFileCatalogUpdate -= UpdateFileCatalog;
        }

        private void UpdateFileCatalog(object sender, List<LockerObject> catalog)
        {
            var img = Resources.text_file;
            var newModel = new ObservableCollection<FileItemViewModel>(catalog.Select(f => new FileItemViewModel(f) { Image = img, Name = f.name }));
            Files = newModel;
        }



        public ExplorerViewModel()
        {
            BackCommand = new RelayCommand(p =>
            {
                NavigationController.Instance.DropViews();
                NavigationController.Instance.ShowView<LoginViewModel>("start");
                BackendConnector.Instance.Init();
            });

            CreateFileCommand = new RelayCommand(p =>
            {
                var dialog = NavigationController.Instance.ShowView<CreateFileViewModel>("create-file");
                dialog.CompleteAction = async (r, f, pw) =>
                {
                    if (r != null)
                    {
                        
                        var error = await BackendConnector.Instance.RequestFileCreate(r, f, pw);
                        if (error != null)
                        {
                            var msg = NavigationController.Instance.ShowView<MessageViewModel>("error-message");
                            msg.Message = error;
                            msg.ButtonMessage = Resources.Ok;
                            msg.MessageActionCommand = new RelayCommand(param =>
                            {
                                NavigationController.Instance.ShowView<CreateFileViewModel>("create-file");
                                NavigationController.Instance.DeleteView<MessageViewModel>("error-message");
                            });
                            return;
                        }
                    }
                    NavigationController.Instance.ShowView<ExplorerViewModel>("home");
                    NavigationController.Instance.DeleteView<CreateFileViewModel>("create-file");
                };
            });

            DeleteFileCommand = new RelayCommand(p =>
            {
                var fileInfo = new LockerObject() {name = _selectedFile.Name}; 
                BackendConnector.Instance.RequestFileDelete(fileInfo);
            }, p => { return Files?.Count(f => f.IsSelected) == 1; });
            BackendConnector.Instance.OnFileCatalogUpdate += UpdateFileCatalog;
            BackendConnector.Instance.RequestFileCatalog();
        }


    }
}
