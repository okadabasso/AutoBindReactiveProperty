using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.ComponentModel;
using BlankApp1.Models;

namespace BlankApp1.ViewModels
{
    public class SampleViewModel : BindableBase
    {
        SampleItemObservable theModel;
        public ReactiveCommand SubmitCommand { get; set; } = new ReactiveCommand();
        public SampleItemViewModel ItemViewModel { get; set; }
        
        
        public SampleViewModel()
        {
            theModel = new SampleItemObservable
            {
                Id = 101,
                Name = "name 101",
                UpdateDateTime = new DateTime(),
                Flag = true
            };

            ItemViewModel = new SampleItemViewModel(theModel);



            SubmitCommand.Subscribe(() => Submit());
        }

        private void Submit()
        {

        }
    }
}
