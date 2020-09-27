using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq.Expressions;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.ComponentModel;
using BlankApp1.ReactiveBinding;
using BlankApp1.Models;

namespace BlankApp1.ViewModels
{
    public class SampleItemViewModel : BindableBase
    {
        [AutoBindingProperty]
        public ReactiveProperty<int> Id { get; set; }
        [AutoBindingProperty]
        public ReactiveProperty<string> Name { get; set; }
        [AutoBindingProperty]
        public ReactiveProperty<DateTime?> UpdateDateTime { get; set; }
        [AutoBindingProperty]
        public ReactiveProperty<bool> Flag { get; set; }

        public SampleItemViewModel()
        {

        }

        public SampleItemViewModel(SampleItemModel model)
        {
            var binder = new ReactiveBinder<SampleItemModel, SampleItemViewModel>();
            binder.Bind(model, this);
        }
        public SampleItemViewModel(SampleItemObservable model)
        {
            var binder = new ReactiveBinder<SampleItemObservable, SampleItemViewModel>();
            binder.BindObservable(model, this);
        }

    }



}
