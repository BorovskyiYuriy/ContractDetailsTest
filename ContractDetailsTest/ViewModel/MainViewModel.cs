using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ContractDetailsTest.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public string ServiceName { get; set; }
        public string SelectedServiceType { get; set; } = null;
        public string SelectedCurrencyType { get; set; } = null;

        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(1);

        public bool IsTrial { get; set; }

        private int _cost;
        public int Cost
        {
            get => _cost;
            set
            {
                _cost = value;
                SplitModels.ToList().ForEach(x => x.UpdateValue());
            }
        }

        public IEnumerable<ServiceType> ServiceTypes => Enum.GetValues(typeof(ServiceType)).Cast<ServiceType>();
        public IEnumerable<CurrencyType> CurrencyTypes => Enum.GetValues(typeof(CurrencyType)).Cast<CurrencyType>();
        public IEnumerable<Department> DepartmentTypes => Enum.GetValues(typeof(Department)).Cast<Department>();

        public List<AllocationModel> AllocationModels { get; set; }
        public ObservableCollection<SplitModel> SplitModels { get; set; }

        public string Comment { get; set; }

        private RelayCommand _saveCommand;
        private RelayCommand _cancelCommand;
        private RelayCommand _addSplitCommand;
        private RelayCommand<AllocationModel> _changeAllocationCommand;
        private RelayCommand<SplitModel> _splitPercentChangedCommand;

        private bool _saveIsPossible;

        public RelayCommand SaveCommand => _saveCommand;
        public RelayCommand CancelCommand => _cancelCommand;
        public RelayCommand AddSplitCommand => _addSplitCommand;
        public RelayCommand<SplitModel> SplitPercentChangedCommand => _splitPercentChangedCommand;
        public RelayCommand<AllocationModel> ChangeAllocationCommand => _changeAllocationCommand;

        public bool IsOpen { get; set; }
        public string ResultText { get; set; }

        public MainViewModel()
        {
            AllocationModels = new List<AllocationModel>();
            Enum.GetValues(typeof(Department)).Cast<Department>().ToList().ForEach(x => AllocationModels.Add(new AllocationModel(x)));
            SplitModels = new ObservableCollection<SplitModel>();

            _saveCommand = new RelayCommand(()  =>
            {
                StringBuilder resultMsg = new StringBuilder();

                resultMsg.AppendLine($"ServiceName: {ServiceName}");
                resultMsg.AppendLine($"ServiceType: {SelectedServiceType}");
                resultMsg.AppendLine($"ServiceCurrency: {SelectedCurrencyType}");
                resultMsg.AppendLine($"IsTrial: {IsTrial}");
                resultMsg.AppendLine($"Cost: {Cost}");
                resultMsg.AppendLine($"StartDate: {StartDate}");
                resultMsg.AppendLine($"EndDate: {EndDate}");
                resultMsg.AppendLine($"Comment: {Comment}");

                ResultText = resultMsg.ToString();
                IsOpen = true;

                RaisePropertyChanged(() => ResultText);
                RaisePropertyChanged(() => IsOpen);
            });

            _cancelCommand = new RelayCommand(() =>
            {
                System.Windows.Application.Current.Shutdown();
            });

            _splitPercentChangedCommand = new RelayCommand<SplitModel>(model =>
            {
                if (model == null)
                    return;

                var currentDep = model.Department;
                var currDepSplits = SplitModels.Where(x => x.Department == currentDep);
                var currDepSplitsPercSum = currDepSplits.Sum(x => x.Percent);

                if (currDepSplitsPercSum > 100)
                {
                    var currDepSplitsExceptChangedSum = currDepSplits.Except(new[] { model }).Sum(x => x.Percent);
                    model.Percent = 100 - currDepSplitsExceptChangedSum;
                }
            });

            _changeAllocationCommand = new RelayCommand<AllocationModel>(model =>
            {
                if (model == null)
                    return;

                var modelsSum = AllocationModels.ToList().Sum(x => x.Value);
                if (modelsSum > 100)
                    model.Value = 100 - AllocationModels.Except(new List<AllocationModel>() { model }).Sum(x => x.Value);

                SplitModels.ToList().ForEach(x => x.UpdateValue());
            });

            _addSplitCommand = new RelayCommand(() =>
            {
                SplitModels.Add(new SplitModel());
            });
        }
    }
}