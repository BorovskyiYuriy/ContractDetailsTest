using CommonServiceLocator;
using ContractDetailsTest.ViewModel;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractDetailsTest
{
    public class SplitModel : ObservableObject
    {
        private MainViewModel _mainVM => ServiceLocator.Current.GetInstance<MainViewModel>();

        private Department _department;
        public Department Department
        {
            get => _department;
            set
            {
                _department = value;
                RaisePropertyChanged(() => Amount);
            }
        }

        private int _percent;
        public int Percent
        {
            get => _percent;
            set
            {
                _percent = value;
                RaisePropertyChanged(() => Percent);
                RaisePropertyChanged(() => Amount);
            }
        }

        private decimal _amount;
        public decimal Amount
        {
            get
            {
                var departmentTotalPercent = _mainVM.AllocationModels.Where(x => x.Department == this.Department).FirstOrDefault();
                if (departmentTotalPercent == null)
                    _amount = 0;
                else
                {
                    decimal departmentTotalAmount = ((decimal)departmentTotalPercent.Value / 100) * _mainVM.Cost;
                    var percentAmount = ((decimal)this.Percent / 100) * departmentTotalAmount;
                    _amount = percentAmount;
                }

                return Math.Round(_amount, 2);
            }
        }

        internal void UpdateValue()
        {
            RaisePropertyChanged(() => Amount);
        }
    }

    public enum ServiceType
    {
        WebDevelopment,
        MobileDevelopment,
        DesktopDevelopment,
        ServiceDevelopment,
        DatabaseArchitecture
    }

    public enum CurrencyType
    {
        USD,
        Euro,
        Rub,
        HRN
    }

    public enum Department
    {
        Development,
        QA,
        Management
    }
}
