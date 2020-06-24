using MVVM.Properties;
using MVVM.Service.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
namespace MVVM.ViewModel
{
    public class AllCustomersViewModel : WorkspaceViewModel, IViewModel
    {
        #region Fields
        readonly CustomerRepository _customerRepository;
        #endregion // Field

        #region Constructor
        public AllCustomersViewModel(CustomerRepository customerRepository)
        {
            base.DisplayName = Resources.AuthenticationViewModel_Command_AllCustomers;
            _customerRepository = customerRepository ?? throw new ArgumentNullException("customerRepository");
            _customerRepository.CustomerAdded += this.OnCustomerAddedToRepository;
            this.CreateAllCustomers();
        }
        void CreateAllCustomers()
        {
            List<CustomerViewModel> all =
                (from cust in _customerRepository.GetCustomers()
                 select new CustomerViewModel(cust, _customerRepository)).ToList();
            foreach (CustomerViewModel cvm in all)
                cvm.PropertyChanged += this.OnCustomerViewModelPropertyChanged;
            this.AllCustomers = new ObservableCollection<CustomerViewModel>(all);
            this.AllCustomers.CollectionChanged += this.OnCollectionChanged;
        }
        #endregion // Constructor

        #region Public Interface
        public ObservableCollection<CustomerViewModel> AllCustomers { get; private set; }
        public double TotalSelectedSales
        {
            get { return this.AllCustomers.Sum(custVM => custVM.IsSelected ? custVM.TotalSales : 0.0); }
        }
        #endregion // Public Interface

        #region  Base Class Overrides
        protected override void OnDispose()
        {
            foreach (CustomerViewModel custVM in this.AllCustomers)
                custVM.Dispose();
            this.AllCustomers.Clear();
            this.AllCustomers.CollectionChanged -= this.OnCollectionChanged;
            _customerRepository.CustomerAdded -= this.OnCustomerAddedToRepository;
        }
        #endregion // Base Class Overrides

        #region Event Handling Methods
        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (CustomerViewModel custVM in e.NewItems)
                    custVM.PropertyChanged += this.OnCustomerViewModelPropertyChanged;
            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (CustomerViewModel custVM in e.OldItems)
                    custVM.PropertyChanged -= this.OnCustomerViewModelPropertyChanged;
        }
        void OnCustomerViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string IsSelected = "IsSelected";
            (sender as CustomerViewModel).VerifyPropertyName(IsSelected);
            if (e.PropertyName == IsSelected)
                this.OnPropertyChanged("TotalSelectedSales");
        }
        void OnCustomerAddedToRepository(object sender, CustomerAddedEventArgs e)
        {
            var viewModel = new CustomerViewModel(e.NewCustomer, _customerRepository);
            this.AllCustomers.Add(viewModel);
        }
        #endregion // Event Handling Methods
    }
}