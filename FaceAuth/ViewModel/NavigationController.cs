using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FaceAuth.ViewModel
{
    /// <summary>
    /// singleton which controlls view navigation
    /// </summary>
    class NavigationController
    {
        /// <summary>
        /// implementation of the singleton pattern
        /// </summary>
        #region singleton impl
        public static NavigationController Instance { get; } = new NavigationController();
        protected NavigationController() { }
        #endregion

        private IViewRoot _viewRoot;

        /// <summary>
        /// dictionary which contains all stored views
        /// </summary>
        private Dictionary<string, BaseViewModel> _views = new Dictionary<string, BaseViewModel>();



        /// <summary>
        /// View which contains the subviews this controller navigates
        /// </summary>
        public IViewRoot ViewRoot
        {
            get
            {
                return _viewRoot;
            }

            set
            {
                _viewRoot = value;
            }
        }


        /// <summary>
        /// switches view to view of type T. if a view with name "name" allready exists it will be reused. the state of the view stays persistent during storage
        /// </summary>
        /// <typeparam name="T">type of the view. needs to inherit from BaseViewModel</typeparam>
        /// <param name="name">id of the view. this ist needed to reuse it or get a reference to the view later</param>    
        /// <returns>the new active view</returns>
        public T ShowView<T>(string name) where T : BaseViewModel, new()
        {
            BaseViewModel result = null;
            if (!_views.TryGetValue(name, out result) || result?.GetType() != typeof(T))  //check if view with id name and type T is stored
            {
                if (_views.ContainsKey(name))
                    throw new ArgumentException("bad name: view with same name but different type already stored");
                result = new T { WindowProperties = ViewRoot.WindowProperties };    //if not, create a new one and add it into the view storage
                _views.Add(name, result);
            }
            ViewRoot.CurrentView = result; //put the new active view in the container
            result.Init();                                                                      //initialize the view
            Task.Delay(50).Wait();                                                              //it seems like INotifyPropertyChanged doesn't work when you change to many properties 
            return (T)result;                                                                   //too fast. so we wait  a little, you won't even notice it
        }

        public T GetView<T>(string name) where T : BaseViewModel
        {
            BaseViewModel result;
            if (_views.TryGetValue(name, out result) && result.GetType() == typeof(T))
            {
                return (T)result;
            }
            throw new ArgumentException("no view of this name and type is stored");
        }

        public void DeleteView<T>(string name) where T : BaseViewModel
        {
            BaseViewModel target;
            if (_views.TryGetValue(name, out target) && target.GetType() == typeof(T))
            {
                target.Stop();
                _views.Remove(name);
                GC.Collect();
            }
            else
            {
                throw new ArgumentException("no view of this name and type is stored");
            }
        }

        public void DropViews()
        {
            var views = new Dictionary<string, BaseViewModel>(_views);
            foreach (var view in views.Where(v => v.Value != _viewRoot))
            {
                view.Value.Stop();
            }
            _views = new Dictionary<string, BaseViewModel>();
            views = null;
            GC.Collect();
        }
    }
}
