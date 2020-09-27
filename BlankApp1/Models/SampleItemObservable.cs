using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace BlankApp1.Models
{
    public class SampleItemObservable : BindableBase
    {
        private int _id;
        private string _name;
        private DateTime? _updateDateTime;
        private bool _flag;
        
        public int Id { get => _id; set => SetProperty(ref _id, value); }
        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public DateTime? UpdateDateTime { get => _updateDateTime; set => SetProperty(ref _updateDateTime, value); }
        public bool Flag { get => _flag; set => SetProperty(ref _flag, value); }
    }
}
