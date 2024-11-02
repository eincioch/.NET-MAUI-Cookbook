﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace c4_LocalDatabaseConnection.ViewModels {
    //public partial class CustomerEditViewModel : CustomerDetailViewModel {
    //    [ObservableProperty]
    //    bool isNewItem;

    //    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    //    [ObservableProperty]
    //    bool isEmailValid;

    //    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    //    [ObservableProperty]
    //    bool isFirstNameValid;

    //    [RelayCommand(CanExecute = nameof(CanSave))]
    //    async Task SaveAsync() {
    //        using var uof = new CrmUnitOfWork();
    //        if (IsNewItem)
    //            await uof.Items.AddAsync(Item);
    //        else
    //            await uof.Items.UpdateAsync(Item);
    //        try {
    //            await uof.SaveAsync();
    //        }
    //        catch (DbUpdateException ex) when (ex.InnerException is SqliteException sqliteEx) {
    //            await Shell.Current.DisplayAlert("Error", sqliteEx.Message, "OK");
    //            return;
    //        }
    //        catch (Exception ex) {
    //            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
    //            return;
    //        }
    //        await ParentRefreshAction(Item);
    //        await Shell.Current.GoToAsync("..");
    //    }
    //    bool CanSave() => IsEmailValid && IsFirstNameValid;
    //    public override void ApplyQueryAttributes(IDictionary<string, object> query) {
    //        if (query.TryGetValue("IsNewItem", out object isNew)) {
    //            IsNewItem = (bool)isNew;
    //        }
    //        base.ApplyQueryAttributes(query);
    //    }
    //}


    public partial class CustomerEditViewModel : CustomerDetailViewModel {
        [ObservableProperty]
        bool isNewItem;

        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        [ObservableProperty]
        bool isEmailValid;

        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        [ObservableProperty]
        bool isFirstNameValid;

        [RelayCommand(CanExecute = nameof(CanSave))]
        async Task SaveAsync() {
            using var uof = new CrmUnitOfWork();
            try {
                if (IsNewItem)
                    await uof.Items.AddAsync(Item);
                else
                    await uof.Items.UpdateAsync(Item);
                await uof.SaveAsync();
            }
            catch (Exception ex) {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                return;
            }
            await ParentRefreshAction(Item);
            await Shell.Current.GoToAsync("..");
        }
        bool CanSave() => IsEmailValid && IsFirstNameValid;
        public override void ApplyQueryAttributes(IDictionary<string, object> query) {
            if (query.TryGetValue("IsNewItem", out object isNew)) {
                IsNewItem = (bool)isNew;
            }
            base.ApplyQueryAttributes(query);
        }
    }
}
