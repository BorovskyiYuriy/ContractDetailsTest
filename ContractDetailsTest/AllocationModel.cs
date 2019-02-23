using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractDetailsTest
{
    public class AllocationModel : ObservableObject
    {
        public string Title { get; set; }

        private int _value;
        public int Value
        {
            get => _value;
            set => Set(() => Value, ref _value, value);
        }

        public Department Department { get; set; }

        public AllocationModel()
        {
            Value = 0;
        }

        public AllocationModel(Department department) : this()
        {
            Department = department;
            Title = $"{department} department";
        }
    }
}
